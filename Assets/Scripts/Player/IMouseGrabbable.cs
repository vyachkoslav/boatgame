namespace Player
{
    public interface IMouseGrabbable : IGrabbable
    {
        public void Hover();
        public void Unhover();
    }
}