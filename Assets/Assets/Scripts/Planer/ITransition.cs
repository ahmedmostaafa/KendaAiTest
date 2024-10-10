namespace KendaAi.Agents.Planer
{
    internal interface ITransition
    {
        IState To { get; }
        IPredicate Condition { get; }
    }
}