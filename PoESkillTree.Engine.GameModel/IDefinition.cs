namespace PoESkillTree.Engine.GameModel
{
    public interface IDefinition<out T>
    {
        T Id { get; }
    }
}