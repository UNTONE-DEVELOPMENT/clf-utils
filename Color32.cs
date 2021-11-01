namespace clf
{
    public class Color32
    {
#pragma warning disable IDE1006 // Naming Styles
        public byte r;
        public byte g;
        public byte b;
        public byte a;
#pragma warning restore IDE1006 // Naming Styles

        public Color32(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }
}
