using Executor.GameQueries;

using System;

namespace Executor.AI
{
    [Serializable()]
    public abstract class Condition : SingleClause
    {
        public abstract bool IsMet(GameQuery_Command commandQuery);
    }
}
