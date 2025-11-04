using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Backend.Object.Character.Player
{
    public partial class AdvancedActionController
    {
        private Dictionary<State, List<Node>> _states;

        private void InitializeStates()
        {
            _states = new Dictionary<State, List<Node>>
            {
                [State.Grounded] = new()
                {
                    new Node
                    {
                        Next = State.Rising,
                        Condition = IsRising,
                        Transition = TranslateToAirborne
                    },
                    new Node
                    {
                        Next = State.Falling,
                        Condition = ShouldFall,
                        Transition = TranslateToAirborne
                    },
                    new Node
                    {
                        Next = State.Sliding,
                        Condition = ShouldSlide,
                        Transition = TranslateToAirborne
                    }
                },
                [State.Sliding] = new()
                {
                    new Node
                    {
                        Next = State.Rising,
                        Condition = IsRising,
                        Transition = TranslateToAirborne
                    },
                    new Node
                    {
                        Next = State.Falling,
                        Condition = ShouldFall,
                        Transition = TranslateToAirborne
                    },
                    new Node
                    {
                        Next = State.Grounded,
                        Condition = () => Composer.MovementController.IsGrounded && ShouldSlide() == false,
                        Transition = TranslateToGrounded
                    }
                },
                [State.Falling] = new()
                {
                    new Node
                    {
                        Next = State.Rising,
                        Condition = IsRising,
                        Transition = null
                    },
                    new Node
                    {
                        Next = State.Grounded,
                        Condition = () => Composer.MovementController.IsGrounded && ShouldSlide() == false,
                        Transition = TranslateToGrounded
                    },
                    new Node { Next = State.Sliding, Condition = ShouldSlide }
                },
                [State.Rising] = new()
                {
                    new Node
                    {
                        Next = State.Grounded,
                        Condition = () => IsRising() == false && Composer.MovementController.IsGrounded && ShouldSlide() == false,
                        Transition = TranslateToGrounded
                    },
                    new Node
                    {
                        Next = State.Sliding,
                        Condition = () => IsRising() == false && ShouldSlide(),
                        Transition = null
                    },
                    new Node
                    {
                        Next = State.Falling,
                        Condition = () => IsRising() == false && ShouldFall(),
                        Transition = null
                    },
                    new Node
                    {
                        Next = State.Falling,
                        Condition = () => false,
                        Transition = ContractCeiling
                    }
                }
            };
        }

        /// <summary>
        /// Determine current controller state based on current momentum and whether the controller is grounded (or not).
        /// </summary>
        private State DetermineState()
        {
            if (_states.ContainsKey(State) == false)
            {
                return State;
            }

            foreach (Node transition in _states[State].Where(transition => transition.Condition()))
            {
                transition.Transition?.Invoke();

                return transition.Next;
            }

            return State;
        }

        /// <returns>
        /// True if vertical momentum is above a small threshold. otherwise false.
        /// </returns>
        private bool IsAirborne()
        {
            // Set up threshold to check against.
            // For most applications, a value of '0.001f' is recommended.
            const float limit = 0.001f;

            // Calculate current vertical momentum.
            var momentum = ConvertMomentumToWorldSpace().Project(transform.up);

            // Return true if vertical momentum is above limit.
            return momentum.magnitude > limit;
        }

        /// <returns>
        /// True if angle between controller and ground normal is too big (greater than slope limit), i.e. ground is too steep. otherwise false.
        /// </returns>
        private bool IsGroundTooSteep()
        {
            if (Composer.MovementController.IsGrounded == false)
            {
                return true;
            }

            return Vector3.Angle(Composer.MovementController.GetGroundNormal(), transform.up) > SlopeLimit;
        }

        private bool IsRising()
        {
            return IsAirborne() && Vector3.Dot(ConvertMomentumToWorldSpace(), transform.up.normalized) > 0f;
        }

        private bool ShouldSlide()
        {
            return Composer.MovementController.IsGrounded && IsGroundTooSteep();
        }

        private bool ShouldFall()
        {
            return Composer.MovementController.IsGrounded == false;
        }

        public State State { get; set; } = State.Falling;

        /// <returns>
        /// True if controller is grounded (or sliding down a slope).
        /// </returns>
        public bool IsGrounded => State is State.Grounded or State.Sliding or State.Rolling or State.Attacking;

        /// <returns>
        /// True if controller is sliding.
        /// </returns>
        public bool IsSliding() => State == State.Sliding;

        public bool IsRolling => State is State.Rolling;

        #region NESTED STRUCTURE API

        private class Node
        {
            public State Next { get; init; }

            public Func<bool> Condition { get; init; }

            public Action Transition { get; init; }
        }

        #endregion
    }
}
