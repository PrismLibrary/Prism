using System;
using System.Windows.Input;

namespace Prism.Services
{
    /// <summary>
    /// ActionSheetButton Base class
    /// </summary>
    public abstract class ActionSheetButtonBase : IActionSheetButton
    {
        protected ActionSheetButtonBase()
        {
            
        }

        /// <summary>
        /// <see cref="ICommand"/> backing <see cref="IActionSheetButton"/>'s Command property
        /// </summary>
        /// <value>The command.</value>
        protected ICommand _command { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IActionSheetButton"/>
        /// is cancel.
        /// </summary>
        /// <value><c>true</c> if is cancel; otherwise, <c>false</c>.</value>
        protected bool _isCancel { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IActionSheetButton"/>
        /// is destroy.
        /// </summary>
        /// <value><c>true</c> if is destroy; otherwise, <c>false</c>.</value>
        protected bool _isDestroy { get; private set; }

        /// <summary>
        /// The backing text for <see cref="IActionSheetButton"/>
        /// </summary>
        /// <value>The text.</value>
        protected string _text { get; private set; }

        /// <summary>
        /// Command to execute when the button is pressed
        /// </summary>
        /// <value>The command.</value>
        [Obsolete("IActionSheetButton is replacing Commands with Action's. Commands will be removed in a future release.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public ICommand Command
        {
            get { return _command; }
            protected internal set { _command = value; }
        }

        /// <summary>
        /// The button will be used as a Cancel Button
        /// </summary>
        /// <value><c>true</c> if is cancel; otherwise, <c>false</c>.</value>
        public bool IsCancel
        {
            get { return _isCancel; }
            protected internal set
            {
                if( _isCancel = value )
                    IsDestroy = false;
            }
        }

        /// <summary>
        /// The button will be used as a Destroy Button
        /// </summary>
        /// <value><c>true</c> if is destroy; otherwise, <c>false</c>.</value>
        public bool IsDestroy
        {
            get { return _isDestroy; }
            protected internal set 
            {
                if( _isDestroy = value )
                    IsCancel = false;
            }
        }

        /// <summary>
        /// Executes the action to take when the button is pressed
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return _text; }
            protected internal set { _text = value; }
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        protected abstract void OnButtonPressed();

        /// <inheritDoc />
        bool IActionSheetButton.IsCancel
        {
            get { return _isCancel; }
        }

        /// <inheritDoc />
        bool IActionSheetButton.IsDestroy
        {
            get { return _isDestroy; }
        }

        /// <inheritDoc />
        string IActionSheetButton.Text
        {
            get { return _text; }
        }

        /// <inheritDoc />
        void IActionSheetButton.PressButton() =>
           OnButtonPressed();
    }
}
