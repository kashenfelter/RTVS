﻿using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Microsoft.Languages.Editor.Text
{
    /// <summary>
    /// event arguments containing a text view
    /// </summary>
    public class TextViewListenerEventArgs : EventArgs
    {
        public ITextBuffer TextBuffer { get; private set; }
        public ITextView TextView { get; private set; }

        public TextViewListenerEventArgs(ITextView textView, ITextBuffer textBuffer)
        {
            TextBuffer = textBuffer;
            TextView = textView;
        }
    }
}
