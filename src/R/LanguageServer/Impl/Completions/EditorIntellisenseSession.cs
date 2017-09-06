﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.Common.Core.Services;
using Microsoft.Languages.Editor.Completions;
using Microsoft.Languages.Editor.Text;
using Microsoft.R.LanguageServer.Services;

namespace Microsoft.R.LanguageServer.Completions {
    internal sealed class EditorIntellisenseSession : ServiceAndPropertyHolder, IEditorIntellisenseSession {
        public EditorIntellisenseSession(IEditorView view) {
            View = view;
        }

        public T As<T>() where T : class => throw new NotImplementedException();

        public new IServiceContainer Services => base.Services;

        public IEditorView View { get; }
        public bool IsDismissed { get; }

        public event EventHandler Dismissed;
    }
}
