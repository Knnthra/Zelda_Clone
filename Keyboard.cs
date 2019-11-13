namespace EksamenJuni2010
{
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public static class Keyboard
    {
        /// <summary>
        /// Use dll user32.dll to get state of key
        /// </summary>
        /// <param name="keyCode">Key code of key to check</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern short GetKeyState(int keyCode);

        /// <summary>
        /// Check to see if keystate is down
        /// </summary>
        /// <param name="key">Key to look at</param>
        /// <returns>Returns true if key is down</returns>
        public static bool IsKeyDown(Keys key)
        {
            bool isDown = false;
            
            short retVal = GetKeyState((int)key);

            if ((retVal & 0x8000) == 0x8000)
                isDown = true;

            return isDown;
        }
    }
}