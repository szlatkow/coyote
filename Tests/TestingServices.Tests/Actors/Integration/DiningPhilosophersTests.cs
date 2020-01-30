﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Coyote.Actors;
using Microsoft.Coyote.Specifications;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Coyote.TestingServices.Tests.Actors
{
    /// <summary>
    /// A single-process implementation of the dining philosophers problem.
    /// </summary>
    public class DiningPhilosophersTests : BaseTest
    {
        public DiningPhilosophersTests(ITestOutputHelper output)
            : base(output)
        {
        }

        private class Environment : Actor
        {
            private Dictionary<int, ActorId> LockMachines;

            protected override Task OnInitializeAsync(Event initialEvent)
            {
                this.LockMachines = new Dictionary<int, ActorId>();

                int n = 3;
                for (int i = 0; i < n; i++)
                {
                    var lck = this.CreateActor(typeof(Lock));
                    this.LockMachines.Add(i, lck);
                }

                for (int i = 0; i < n; i++)
                {
                    this.CreateActor(typeof(Philosopher), new Philosopher.SetupEvent(this.LockMachines[i], this.LockMachines[(i + 1) % n]));
                }

                return Task.CompletedTask;
            }
        }

        private class Lock : StateMachine
        {
            public class TryLock : Event
            {
                public ActorId Target;

                public TryLock(ActorId target)
                {
                    this.Target = target;
                }
            }

            public class Release : Event
            {
            }

            public class LockResp : Event
            {
                public bool LockResult;

                public LockResp(bool res)
                {
                    this.LockResult = res;
                }
            }

            private bool LockVar;

            [Start]
            [OnEntry(nameof(InitOnEntry))]
            private class Init : State
            {
            }

            [OnEventDoAction(typeof(TryLock), nameof(OnTryLock))]
            [OnEventDoAction(typeof(Release), nameof(OnRelease))]
            private class Waiting : State
            {
            }

            private Transition InitOnEntry()
            {
                this.LockVar = false;
                return this.GotoState<Waiting>();
            }

            private void OnTryLock(Event e)
            {
                var target = (e as TryLock).Target;
                if (this.LockVar)
                {
                    this.SendEvent(target, new LockResp(false));
                }
                else
                {
                    this.LockVar = true;
                    this.SendEvent(target, new LockResp(true));
                }
            }

            private void OnRelease()
            {
                this.LockVar = false;
            }
        }

        private class Philosopher : StateMachine
        {
            public class SetupEvent : Event
            {
                public ActorId Left;
                public ActorId Right;

                public SetupEvent(ActorId left, ActorId right)
                {
                    this.Left = left;
                    this.Right = right;
                }
            }

            private class TryAgain : Event
            {
            }

            private ActorId left;
            private ActorId right;

            [Start]
            [OnEntry(nameof(InitOnEntry))]
            private class Init : State
            {
            }

            [OnEntry(nameof(TryAccess))]
            [OnEventDoAction(typeof(TryAgain), nameof(TryAccess))]
            private class Trying : State
            {
            }

            [OnEntry(nameof(OnDone))]
            private class Done : State
            {
            }

            private Transition InitOnEntry(Event e)
            {
                this.left = (e as SetupEvent).Left;
                this.right = (e as SetupEvent).Right;
                return this.GotoState<Trying>();
            }

            private async Task<Transition> TryAccess()
            {
                this.SendEvent(this.left, new Lock.TryLock(this.Id));
                var ev = await this.ReceiveEventAsync(typeof(Lock.LockResp));
                if ((ev as Lock.LockResp).LockResult)
                {
                    this.SendEvent(this.right, new Lock.TryLock(this.Id));
                    var evr = await this.ReceiveEventAsync(typeof(Lock.LockResp));
                    if ((evr as Lock.LockResp).LockResult)
                    {
                        return this.GotoState<Done>();
                    }
                    else
                    {
                        this.SendEvent(this.left, new Lock.Release());
                    }
                }

                this.SendEvent(this.Id, new TryAgain());
                return default;
            }

            private Transition OnDone()
            {
                this.SendEvent(this.left, new Lock.Release());
                this.SendEvent(this.right, new Lock.Release());
                this.Monitor<LivenessMonitor>(new LivenessMonitor.NotifyDone());
                return this.Halt();
            }
        }

        private class LivenessMonitor : Monitor
        {
            public class NotifyDone : Event
            {
            }

            [Start]
            [Hot]
            [OnEventGotoState(typeof(NotifyDone), typeof(Done))]
            private class Init : State
            {
            }

            [Cold]
            [OnEventGotoState(typeof(NotifyDone), typeof(Done))]
            private class Done : State
            {
            }
        }

        [Theory(Timeout = 10000)]
        [InlineData(469)]
        public void TestDiningPhilosophersLivenessBugWithCycleReplay(int seed)
        {
            var configuration = GetConfiguration();
            configuration.EnableCycleDetection = true;
            configuration.MaxUnfairSchedulingSteps = 100;
            configuration.MaxFairSchedulingSteps = 1000;
            configuration.LivenessTemperatureThreshold = 500;
            configuration.RandomSchedulingSeed = seed;
            configuration.SchedulingIterations = 1;

            this.TestWithError(r =>
            {
                r.RegisterMonitor(typeof(LivenessMonitor));
                r.CreateActor(typeof(Environment));
            },
            configuration: configuration,
            expectedError: "LivenessMonitor detected infinite execution that violates a liveness property.",
            replay: true);
        }
    }
}