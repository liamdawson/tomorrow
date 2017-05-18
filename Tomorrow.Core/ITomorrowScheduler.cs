using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Tomorrow.Core
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    public interface ITomorrowScheduler
    {
        Task Schedule(MethodInfo method, params object[] parameters);
        Task Schedule(string queueName, MethodInfo method, params object[] parameters);
        Task Schedule<T>(Expression<Action<T>> simpleExpression);
        Task Schedule<T>(string queueName, Expression<Action<T>> simpleExpression);
    }
}