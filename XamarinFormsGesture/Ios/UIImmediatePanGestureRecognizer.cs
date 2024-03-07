using System;
using Foundation;
using UIKit;

namespace Vapolia.Lib.Effects
{
    internal class UIImmediatePanGestureRecognizer : UIPanGestureRecognizer
    {
        public bool IsImmediate { get; set; } = false;

        public UIImmediatePanGestureRecognizer()
        {
        }

        public UIImmediatePanGestureRecognizer(Action action) : base(action)
        {
        }

        public UIImmediatePanGestureRecognizer(Action<UIPanGestureRecognizer> action) : base(action)
        {
        }

        [Preserve]
        protected internal UIImmediatePanGestureRecognizer(IntPtr handle) : base(handle)
        {
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            if (IsImmediate)
                State = UIGestureRecognizerState.Began;
        }
    }
}