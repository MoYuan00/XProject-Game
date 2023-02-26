namespace Framework
{
    public interface IState
    {
        public IState Enter();

        public IState Update();

        public IState FixUpdate();

        public IState Exit();
    }
}