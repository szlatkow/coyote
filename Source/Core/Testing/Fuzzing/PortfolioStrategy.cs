﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Coyote.Testing.Fuzzing
{
    internal class PortfolioStrategy : FuzzingStrategy
    {
        /// <summary>
        /// Random value generator.
        /// </summary>
        protected readonly IRandomValueGenerator RandomValueGenerator;

        /// <summary>
        /// The maximum number of steps to explore.
        /// </summary>
        protected readonly int MaxSteps;

        protected FuzzingStrategy CurrentStrategy;
        protected enum Strategy
        {
            Random,
            TorchRandom,
            FairPCT,
            LowDelayProbability,
            PPCT,
            CoinToss,
            OneStopOneGo
        }

        protected Strategy NextStrategy;

        protected int PriorityChangePoints;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortfolioStrategy"/> class.
        /// </summary>
        internal PortfolioStrategy(IRandomValueGenerator random, int maxDelays, int priorityChangePoint)
        {
            this.RandomValueGenerator = random;
            this.MaxSteps = maxDelays;
            this.PriorityChangePoints = priorityChangePoint;

            this.CurrentStrategy = new RandomStrategy(random, maxDelays);
            this.NextStrategy = Strategy.Random;
        }

        /// <inheritdoc/>
        internal override bool InitializeNextIteration(uint iteration)
        {
            switch (this.NextStrategy)
            {
                case Strategy.TorchRandom:
                    this.CurrentStrategy = new TorchRandomStrategy(this.RandomValueGenerator, this.MaxSteps);
                    break;
                case Strategy.Random:
                    this.CurrentStrategy = new RandomStrategy(this.RandomValueGenerator, this.MaxSteps);
                    break;
                case Strategy.FairPCT:
                    this.CurrentStrategy = new FairPCTStrategy(this.RandomValueGenerator, this.MaxSteps);
                    break;
                case Strategy.LowDelayProbability:
                    this.CurrentStrategy = new LowDelayPercentageStrategy(this.RandomValueGenerator, this.MaxSteps);
                    break;
                case Strategy.PPCT:
                    this.CurrentStrategy = new PPCTStrategy(this.RandomValueGenerator, this.MaxSteps, this.PriorityChangePoints);
                    break;
                case Strategy.OneStopOneGo:
                    this.CurrentStrategy = new OneStopOneGoStrategy(this.RandomValueGenerator, this.MaxSteps, this.PriorityChangePoints);
                    break;
                case Strategy.CoinToss:
                    this.CurrentStrategy = new CoinTossStrategy(this.RandomValueGenerator, this.MaxSteps);
                    break;

                default:
                    this.CurrentStrategy = new RandomStrategy(this.RandomValueGenerator, this.MaxSteps);
                    this.NextStrategy = Strategy.Random;
                    break;
            }

            this.NextStrategy++;

            return this.CurrentStrategy.InitializeNextIteration(iteration);
        }

        /// <inheritdoc/>
        internal override bool GetNextDelay(int maxValue, out int next)
        {
            return this.CurrentStrategy.GetNextDelay(maxValue, out next);
        }

        /// <inheritdoc/>
        internal override int GetStepCount() => this.CurrentStrategy.GetStepCount();

        /// <inheritdoc/>
        internal override bool IsMaxStepsReached() => this.CurrentStrategy.IsMaxStepsReached();

        /// <inheritdoc/>
        internal override bool IsFair() => this.CurrentStrategy.IsFair();

        /// <inheritdoc/>
        internal override string GetDescription() => this.CurrentStrategy.GetDescription();
    }
}
