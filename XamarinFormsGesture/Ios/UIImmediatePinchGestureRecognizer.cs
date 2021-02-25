using System;
using Foundation;
using UIKit;

namespace Vapolia.Ios.Lib.Effects
{
    public class UIImmediatePinchGestureRecognizer : UIPinchGestureRecognizer
    {
        public bool IsImmediate { get; set; } = false;

        public UIImmediatePinchGestureRecognizer()
        {
        }

        public UIImmediatePinchGestureRecognizer(Action action) : base(action)
        {
        }

        public UIImmediatePinchGestureRecognizer(Action<UIPinchGestureRecognizer> action) : base(action)
        {
        }

        [Preserve]
        protected internal UIImmediatePinchGestureRecognizer(IntPtr handle) : base(handle)
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