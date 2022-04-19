using System;
using System.Reflection;
using ViewModel;

namespace Presenting
{
    public static class BindResolver
    {
        public static TBindData GetBindData<TBindData>(this IViewModel viewModel) where TBindData : IBindData
        {
            return viewModel.GetBindData<TBindData>(Activator.CreateInstance<TBindData>());
        }
        
        public static TBindData GetBindData<TBindData>(this IViewModel viewModel, TBindData bindData) where TBindData : IBindData
        {
            //todo : add code generation binding
            var type = typeof(TBindData);
            object refBindData = bindData;
            foreach (var fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (fieldInfo.GetCustomAttribute(typeof(BindAttribute)) is BindAttribute bindAttribute)
                {
                    fieldInfo.SetValue(refBindData, viewModel.GetViewModelData(bindAttribute.key));
                }   
            }

            return (TBindData) refBindData;
        }

        public static ref TBindData GetBindData<TBindData>(this IViewModel viewModel, ref TBindData bindData)
            where TBindData : struct, IBindData
        {
            var type = typeof(TBindData);
            object refBindData = bindData;
            foreach (var fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (fieldInfo.GetCustomAttribute(typeof(BindAttribute)) is BindAttribute bindAttribute)
                {
                    fieldInfo.SetValue(refBindData, viewModel.GetViewModelData(bindAttribute.key));
                }   
            }

            bindData = (TBindData) refBindData;
            return ref bindData;
        }
    }
}