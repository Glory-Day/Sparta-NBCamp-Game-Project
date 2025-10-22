using System;

namespace Backend.Util.Presentation
{
    public class Presenter<TView, TModel> : IPresenter, IClearable where TView : IView where TModel : IModel
    {
        public Presenter(TView view, TModel model)
        {
            View = view;
            Model = model;
        }

        public virtual void Clear()
        {
            View = default;
            Model = default;
        }

        protected Action EventHandler;

        protected TView View { get; set; }

        protected TModel Model { get; set; }
    }
}
