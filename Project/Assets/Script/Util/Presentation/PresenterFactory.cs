using System;
using Backend.Util.Debug;

namespace Backend.Util.Presentation
{
    public static class PresenterFactory
    {
        public static T Create<T>(params object[] data) where T : IPresenter
        {
            T presenter = default;

            var isExisted = false;
            var type = typeof(T);
            var information = type.GetConstructors();
            var length = information.Length;

            for (var i = 0; i < length; i++)
            {
                if (information[i].GetParameters().Length != data.Length)
                {
                    continue;
                }

                isExisted = true;
                break;
            }

            if (isExisted)
            {
                presenter = (T)Activator.CreateInstance(type, data);
            }
            else
            {
                var name = type.Name;
                Debugger.LogError($" Cannot create {name}. Count of items in data doesn't match with counts of arguments of {name}'s constructor.");
            }

            return presenter;
        }
    }
}
