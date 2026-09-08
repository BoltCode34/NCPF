using NCPF.Domain;
using System.Threading.Tasks;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// A composite goal: assembles <see cref="GoalBase"/> from TWO independent halves — achievement
    /// and heuristic. Split on purpose (they are orthogonal; combining them into goal classes would
    /// be a combinatorial explosion of N achievements × M heuristics). Both halves are swapped
    /// separately; consistency is on whoever pairs them.
    /// </summary>
    public class BaseGoalBuilder : IGoalBuilder
    {
        public IHeuristicBuilder HeuristicBuilder;
        public IAchivementBuilder AchivementBuilder;

        public IGoal Create(Config start, Config end)
        {
            return new GoalBase(
                AchivementBuilder.Create(start, end),
                HeuristicBuilder.Create(start, end));
        }

        async Task<IGoal> IAsyncGoalBuilder.Create(Config start, Config end)
        {
            return new GoalBase(
                await ((IAsyncAchivementBuilder)AchivementBuilder).Create(start, end),
                await ((IAsyncHeuristicBuilder)HeuristicBuilder).Create(start, end));
        }
    }
}
