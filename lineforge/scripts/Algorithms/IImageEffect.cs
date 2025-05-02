using Godot;

namespace LineForge.Algorithms
{
    public interface IImageEffect
    {
        Image ProcessImage(Image input);
    }
}