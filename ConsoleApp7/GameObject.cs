partial class Program //классы без наследования + примерно тут я мы дошли до изучения геттеров и сеттеров, бож, ради всего святого, простите, но мне так лень их сюда вписывать....
{
    public class GameObject
    {
        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                PrePosition = _position;
                _position = value;
            }
        }
        public GameObject(char symbol, Vector2 position, bool passable)
        {
            Symbol = symbol;
            Position = position;
            Passable = passable;
            PrePosition = new Vector2(_position.X, _position.Y);
        }
        public Vector2 _position;
        public Vector2 PrePosition { get; set; }
        public bool Passable { get; set; }
        public char Symbol { get; set; }
        public event Action<GameObject> OnDestroy;
        public event Action<GameObject, Vector2> OnMove;
        public virtual void Update()
        {
        }
        protected virtual void OnDestroyAction()
        {

        }

        public void Destroy()
        {
            OnDestroyAction();
            if (OnDestroy != null)
                OnDestroy(this);
        }
    }
}
