using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Heatwave.Application.Extensions;

public static partial class LambadaExpressionExtensions
{
    static LambdaExpression InternalAndOrExpression(bool condition, LambdaExpression exp1, LambdaExpression exp2, bool isAndAlso)
    {
        if (condition == false) return exp1;
        if (exp1 == null) return exp2;
        if (exp2 == null) return exp1;

        var newParameters = exp1.Parameters.Select((a, b) => Expression.Parameter(a.Type, a.Name /*$"new{b}"*/)).ToArray();

        var left = new NewExpressionVisitor(newParameters, exp1.Parameters.ToArray()).Replace(exp1.Body);
        var right = new NewExpressionVisitor(newParameters, exp2.Parameters.ToArray()).Replace(exp2.Body);
        var body = isAndAlso ? Expression.AndAlso(left, right) : Expression.OrElse(left, right);
        return Expression.Lambda(exp1.Type, body, newParameters);
    }
    static LambdaExpression InternalNotExpression(bool condition, LambdaExpression exp)
    {
        if (condition == false) return exp;
        if (exp == null) return null;

        var newParameters = exp.Parameters.Select((a, b) => Expression.Parameter(a.Type, a.Name /*$"new{b}"*/)).ToArray();
        var body = Expression.Not(exp.Body);
        return Expression.Lambda(exp.Type, body, newParameters);
    }

    private static Expression<TDelegate> GetExtendSelectExpre<TBase, TResult, TDelegate>(Expression<TDelegate> expression, params string[] exclude)
    {

        NewExpression newBody = Expression.New(typeof(TResult));
        MemberInitExpression oldExpression = (MemberInitExpression)expression.Body;
        ParameterExpression[] oldParamters = expression.Parameters.ToArray();
        List<string> exisProperties = oldExpression.Bindings.ToList().Select(a => a.Member.Name).ToList();

        List<PropertyInfo> existsPropertieslist = exisProperties.Count > 0
            ? typeof(TBase).GetProperties().Where(x => !exisProperties.Contains(x.Name) && x.CanWrite).ToList()
            : typeof(TBase).GetProperties().Where(x => x.CanWrite).ToList();
        if (exclude != null && exclude.Length > 0)
        {
            existsPropertieslist = existsPropertieslist.Where(x => !exclude.Contains(x.Name)).ToList();
        }
        List<MemberBinding> newBindings = [];
        MemberBinding newMemberBinding = null;
        MemberExpression valueExpre = null;
        MemberInfo[] ddt = typeof(TResult).GetMembers();
        bool cct = typeof(TBase).IsAssignableFrom(typeof(TResult));
        existsPropertieslist.ForEach(aProperty =>
        {
            if (ddt.Any(x => x.Name == aProperty.Name))
            {
                valueExpre = Expression.Property(oldParamters[0], aProperty.Name);
                newMemberBinding = cct
                    ? Expression.Bind(aProperty, valueExpre)
                    : (MemberBinding)Expression.Bind(typeof(TResult).GetProperty(aProperty.Name), valueExpre);
                newBindings.Add(newMemberBinding);
            }
        });
        newBindings.AddRange(oldExpression.Bindings);

        MemberInitExpression body = Expression.MemberInit(newBody, newBindings.ToArray());
        Expression<TDelegate> resExpression = Expression.Lambda<TDelegate>(body, oldParamters);

        return resExpression;
    }


    /// <summary>
    /// 组合继承属性选择表达式树,无拓展参数 TResult将继承TBase的所有属性
    /// </summary>
    /// <typeparam name="TBase"> 原数据类型 </typeparam>
    /// <typeparam name="TResult"> 返回类型 </typeparam>
    /// <param name="expression"> 拓展表达式 </param>
    /// <param name="exclude"> 排除字段 </param>
    /// <returns> </returns>
    public static Expression<Func<TBase, TResult>> BuildExtendSelectExpre<TBase, TResult>(this Expression<Func<TBase, TResult>> expression, params string[] exclude)
    {
        return GetExtendSelectExpre<TBase, TResult, Func<TBase, TResult>>(expression, exclude);
    }

    #region T1
    /// <summary>
    /// 使用 and 拼接两个 lambda 表达式
    /// </summary>
    /// <returns></returns>
    public static Expression<Func<T1, bool>> And<T1>(this Expression<Func<T1, bool>> exp1, Expression<Func<T1, bool>> exp2) => And(exp1, true, exp2);
    /// <summary>
    /// 使用 and 拼接两个 lambda 表达式
    /// </summary>
    /// <param name="exp1"></param>
    /// <param name="condition">true 时生效</param>
    /// <param name="exp2"></param>
    /// <returns></returns>
    public static Expression<Func<T1, bool>> And<T1>(this Expression<Func<T1, bool>> exp1, bool condition, Expression<Func<T1, bool>> exp2) => (Expression<Func<T1, bool>>)InternalAndOrExpression(condition, exp1, exp2, true);

    /// <summary>
    /// 使用 or 拼接两个 lambda 表达式
    /// </summary>
    /// <returns></returns>
    public static Expression<Func<T1, bool>> Or<T1>(this Expression<Func<T1, bool>> exp1, Expression<Func<T1, bool>> exp2) => Or(exp1, true, exp2);
    /// <summary>
    /// 使用 or 拼接两个 lambda 表达式
    /// </summary>
    /// <param name="exp1"></param>
    /// <param name="condition">true 时生效</param>
    /// <param name="exp2"></param>
    /// <returns></returns>
    public static Expression<Func<T1, bool>> Or<T1>(this Expression<Func<T1, bool>> exp1, bool condition, Expression<Func<T1, bool>> exp2) => (Expression<Func<T1, bool>>)InternalAndOrExpression(condition, exp1, exp2, false);

    /// <summary>
    /// 将 lambda 表达式取反
    /// </summary>
    /// <param name="exp"></param>
    /// <param name="condition">true 时生效</param>
    /// <returns></returns>
    public static Expression<Func<T1, bool>> Not<T1>(this Expression<Func<T1, bool>> exp, bool condition = true) => (Expression<Func<T1, bool>>)InternalNotExpression(condition, exp);
    
    #region 拓展AndIf与AndOr
    /// <summary>
    /// 符合条件则And
    /// </summary>
    /// <typeparam name="T"> 实体类型 </typeparam>
    /// <param name="left"> 原表达式 </param>
    /// <param name="need"> 是否符合条件 </param>
    /// <param name="right"> 新表达式 </param>
    /// <returns> </returns>
    public static Expression<Func<T, bool>> AndIf<T>(this Expression<Func<T, bool>> left, bool need, Expression<Func<T, bool>> right)
    {
        return need ? left.And(right) : left;
    }

    /// <summary>
    /// 符合条件则Or
    /// </summary>
    /// <typeparam name="T"> 实体类型 </typeparam>
    /// <param name="left"> 原表达式 </param>
    /// <param name="need"> 是否符合条件 </param>
    /// <param name="right"> 新表达式 </param>
    /// <returns> </returns>
    public static Expression<Func<T, bool>> OrIf<T>(this Expression<Func<T, bool>> left, bool need, Expression<Func<T, bool>> right)
    {
        return need ? left.Or(right) : left;
    }

    #endregion 拓展AndIf与AndOr

    /// <summary>
    /// 组合继承属性选择表达式树,1个拓展参数 TResult将继承TBase的所有属性
    /// </summary>
    /// <typeparam name="TBase"> 原数据类型 </typeparam>
    /// <typeparam name="T1"> 拓展类型1 </typeparam>
    /// <typeparam name="TResult"> 返回类型 </typeparam>
    /// <param name="expression"> 拓展表达式 </param>
    /// <param name="exclude"> 排除字段 </param>
    /// <returns> </returns>
    public static Expression<Func<TBase, T1, TResult>> BuildExtendSelectExpre<TBase, T1, TResult>(this Expression<Func<TBase, T1, TResult>> expression, params string[] exclude)
    {
        return GetExtendSelectExpre<TBase, TResult, Func<TBase, T1, TResult>>(expression, exclude);
    }

    #endregion

    #region T1, T2
    /// <summary>
    /// 使用 and 拼接两个 lambda 表达式
    /// </summary>
    /// <returns></returns>
    public static Expression<Func<T1, T2, bool>> And<T1, T2>(this Expression<Func<T1, T2, bool>> exp1, Expression<Func<T1, T2, bool>> exp2) => And(exp1, true, exp2);
    /// <summary>
    /// 使用 and 拼接两个 lambda 表达式
    /// </summary>
    /// <param name="exp1"></param>
    /// <param name="condition">true 时生效</param>
    /// <param name="exp2"></param>
    /// <returns></returns>
    public static Expression<Func<T1, T2, bool>> And<T1, T2>(this Expression<Func<T1, T2, bool>> exp1, bool condition, Expression<Func<T1, T2, bool>> exp2) => (Expression<Func<T1, T2, bool>>)InternalAndOrExpression(condition, exp1, exp2, true);

    /// <summary>
    /// 使用 or 拼接两个 lambda 表达式
    /// </summary>
    /// <returns></returns>
    public static Expression<Func<T1, T2, bool>> Or<T1, T2>(this Expression<Func<T1, T2, bool>> exp1, Expression<Func<T1, T2, bool>> exp2) => Or(exp1, true, exp2);
    /// <summary>
    /// 使用 or 拼接两个 lambda 表达式
    /// </summary>
    /// <param name="exp1"></param>
    /// <param name="condition">true 时生效</param>
    /// <param name="exp2"></param>
    /// <returns></returns>
    public static Expression<Func<T1, T2, bool>> Or<T1, T2>(this Expression<Func<T1, T2, bool>> exp1, bool condition, Expression<Func<T1, T2, bool>> exp2) => (Expression<Func<T1, T2, bool>>)InternalAndOrExpression(condition, exp1, exp2, false);

    /// <summary>
    /// 将 lambda 表达式取反
    /// </summary>
    /// <param name="exp"></param>
    /// <param name="condition">true 时生效</param>
    /// <returns></returns>
    public static Expression<Func<T1, T2, bool>> Not<T1, T2>(this Expression<Func<T1, T2, bool>> exp, bool condition = true) => (Expression<Func<T1, T2, bool>>)InternalNotExpression(condition, exp);

    /// <summary>
    /// 组合继承属性选择表达式树,2个拓展参数 TResult将继承TBase的所有属性
    /// </summary>
    /// <typeparam name="TBase"> 原数据类型 </typeparam>
    /// <typeparam name="T1"> 拓展类型1 </typeparam>
    /// <typeparam name="T2"> 拓展类型2 </typeparam>
    /// <typeparam name="TResult"> 返回类型 </typeparam>
    /// <param name="expression"> 拓展表达式 </param>
    /// <param name="exclude"> 排除字段 </param>
    /// <returns> </returns>
    public static Expression<Func<TBase, T1, T2, TResult>> BuildExtendSelectExpre<TBase, T1, T2, TResult>(this Expression<Func<TBase, T1, T2, TResult>> expression, params string[] exclude)
    {
        return GetExtendSelectExpre<TBase, TResult, Func<TBase, T1, T2, TResult>>(expression, exclude);
    }
    #endregion

    #region T1, T2, T3
    /// <summary>
    /// 使用 and 拼接两个 lambda 表达式
    /// </summary>
    /// <returns></returns>
    public static Expression<Func<T1, T2, T3, bool>> And<T1, T2, T3>(this Expression<Func<T1, T2, T3, bool>> exp1, Expression<Func<T1, T2, T3, bool>> exp2) => And(exp1, true, exp2);
    /// <summary>
    /// 使用 and 拼接两个 lambda 表达式
    /// </summary>
    /// <param name="exp1"></param>
    /// <param name="condition">true 时生效</param>
    /// <param name="exp2"></param>
    /// <returns></returns>
    public static Expression<Func<T1, T2, T3, bool>> And<T1, T2, T3>(this Expression<Func<T1, T2, T3, bool>> exp1, bool condition, Expression<Func<T1, T2, T3, bool>> exp2) => (Expression<Func<T1, T2, T3, bool>>)InternalAndOrExpression(condition, exp1, exp2, true);

    /// <summary>
    /// 使用 or 拼接两个 lambda 表达式
    /// </summary>
    /// <returns></returns>
    public static Expression<Func<T1, T2, T3, bool>> Or<T1, T2, T3>(this Expression<Func<T1, T2, T3, bool>> exp1, Expression<Func<T1, T2, T3, bool>> exp2) => Or(exp1, true, exp2);
    /// <summary>
    /// 使用 or 拼接两个 lambda 表达式
    /// </summary>
    /// <param name="exp1"></param>
    /// <param name="condition">true 时生效</param>
    /// <param name="exp2"></param>
    /// <returns></returns>
    public static Expression<Func<T1, T2, T3, bool>> Or<T1, T2, T3>(this Expression<Func<T1, T2, T3, bool>> exp1, bool condition, Expression<Func<T1, T2, T3, bool>> exp2) => (Expression<Func<T1, T2, T3, bool>>)InternalAndOrExpression(condition, exp1, exp2, false);

    /// <summary>
    /// 将 lambda 表达式取反
    /// </summary>
    /// <param name="exp"></param>
    /// <param name="condition">true 时生效</param>
    /// <returns></returns>
    public static Expression<Func<T1, T2, T3, bool>> Not<T1, T2, T3>(this Expression<Func<T1, T2, T3, bool>> exp, bool condition = true) => (Expression<Func<T1, T2, T3, bool>>)InternalNotExpression(condition, exp);


    /// <summary>
    /// 组合继承属性选择表达式树,3个拓展参数 TResult将继承TBase的所有属性
    /// </summary>
    /// <typeparam name="TBase"> 原数据类型 </typeparam>
    /// <typeparam name="T1"> 拓展类型1 </typeparam>
    /// <typeparam name="T2"> 拓展类型2 </typeparam>
    /// <typeparam name="T3"> 拓展类型3 </typeparam>
    /// <typeparam name="TResult"> 返回类型 </typeparam>
    /// <param name="expression"> 拓展表达式 </param>
    /// <param name="exclude"> 排除字段 </param>
    /// <returns> </returns>
    public static Expression<Func<TBase, T1, T2, T3, TResult>> BuildExtendSelectExpre<TBase, T1, T2, T3, TResult>(this Expression<Func<TBase, T1, T2, T3, TResult>> expression, params string[] exclude)
    {
        return GetExtendSelectExpre<TBase, TResult, Func<TBase, T1, T2, T3, TResult>>(expression, exclude);
    }
    #endregion

    #region T1, T2, T3, T4
    /// <summary>
    /// 使用 and 拼接两个 lambda 表达式
    /// </summary>
    /// <returns></returns>
    public static Expression<Func<T1, T2, T3, T4, bool>> And<T1, T2, T3, T4>(this Expression<Func<T1, T2, T3, T4, bool>> exp1, Expression<Func<T1, T2, T3, T4, bool>> exp2) => And(exp1, true, exp2);
    /// <summary>
    /// 使用 and 拼接两个 lambda 表达式
    /// </summary>
    /// <param name="exp1"></param>
    /// <param name="condition">true 时生效</param>
    /// <param name="exp2"></param>
    /// <returns></returns>
    public static Expression<Func<T1, T2, T3, T4, bool>> And<T1, T2, T3, T4>(this Expression<Func<T1, T2, T3, T4, bool>> exp1, bool condition, Expression<Func<T1, T2, T3, T4, bool>> exp2) => (Expression<Func<T1, T2, T3, T4, bool>>)InternalAndOrExpression(condition, exp1, exp2, true);

    /// <summary>
    /// 使用 or 拼接两个 lambda 表达式
    /// </summary>
    /// <returns></returns>
    public static Expression<Func<T1, T2, T3, T4, bool>> Or<T1, T2, T3, T4>(this Expression<Func<T1, T2, T3, T4, bool>> exp1, Expression<Func<T1, T2, T3, T4, bool>> exp2) => Or(exp1, true, exp2);
    /// <summary>
    /// 使用 or 拼接两个 lambda 表达式
    /// </summary>
    /// <param name="exp1"></param>
    /// <param name="condition">true 时生效</param>
    /// <param name="exp2"></param>
    /// <returns></returns>
    public static Expression<Func<T1, T2, T3, T4, bool>> Or<T1, T2, T3, T4>(this Expression<Func<T1, T2, T3, T4, bool>> exp1, bool condition, Expression<Func<T1, T2, T3, T4, bool>> exp2) => (Expression<Func<T1, T2, T3, T4, bool>>)InternalAndOrExpression(condition, exp1, exp2, false);

    /// <summary>
    /// 将 lambda 表达式取反
    /// </summary>
    /// <param name="exp"></param>
    /// <param name="condition">true 时生效</param>
    /// <returns></returns>
    public static Expression<Func<T1, T2, T3, T4, bool>> Not<T1, T2, T3, T4>(this Expression<Func<T1, T2, T3, T4, bool>> exp, bool condition = true) => (Expression<Func<T1, T2, T3, T4, bool>>)InternalNotExpression(condition, exp);

    /// <summary>
    /// 组合继承属性选择表达式树,4个拓展参数 TResult将继承TBase的所有属性
    /// </summary>
    /// <typeparam name="TBase"> 原数据类型 </typeparam>
    /// <typeparam name="T1"> 拓展类型1 </typeparam>
    /// <typeparam name="T2"> 拓展类型2 </typeparam>
    /// <typeparam name="T3"> 拓展类型3 </typeparam>
    /// <typeparam name="T4"> 拓展类型4 </typeparam>
    /// <typeparam name="TResult"> 返回类型 </typeparam>
    /// <param name="expression"> 拓展表达式 </param>
    /// <param name="exclude"> 排除字段 </param>
    /// <returns> </returns>
    public static Expression<Func<TBase, T1, T2, T3, T4, TResult>> BuildExtendSelectExpre<TBase, T1, T2, T3, T4, TResult>(this Expression<Func<TBase, T1, T2, T3, T4, TResult>> expression, params string[] exclude)
    {
        return GetExtendSelectExpre<TBase, TResult, Func<TBase, T1, T2, T3, T4, TResult>>(expression, exclude);
    }
    #endregion

    #region T1, T2, T3, T4, T5
    /// <summary>
    /// 使用 and 拼接两个 lambda 表达式
    /// </summary>
    /// <returns></returns>
    public static Expression<Func<T1, T2, T3, T4, T5, bool>> And<T1, T2, T3, T4, T5>(this Expression<Func<T1, T2, T3, T4, T5, bool>> exp1, Expression<Func<T1, T2, T3, T4, T5, bool>> exp2) => And(exp1, true, exp2);
    /// <summary>
    /// 使用 and 拼接两个 lambda 表达式
    /// </summary>
    /// <param name="exp1"></param>
    /// <param name="condition">true 时生效</param>
    /// <param name="exp2"></param>
    /// <returns></returns>
    public static Expression<Func<T1, T2, T3, T4, T5, bool>> And<T1, T2, T3, T4, T5>(this Expression<Func<T1, T2, T3, T4, T5, bool>> exp1, bool condition, Expression<Func<T1, T2, T3, T4, T5, bool>> exp2) => (Expression<Func<T1, T2, T3, T4, T5, bool>>)InternalAndOrExpression(condition, exp1, exp2, true);

    /// <summary>
    /// 使用 or 拼接两个 lambda 表达式
    /// </summary>
    /// <returns></returns>
    public static Expression<Func<T1, T2, T3, T4, T5, bool>> Or<T1, T2, T3, T4, T5>(this Expression<Func<T1, T2, T3, T4, T5, bool>> exp1, Expression<Func<T1, T2, T3, T4, T5, bool>> exp2) => Or(exp1, true, exp2);
    /// <summary>
    /// 使用 or 拼接两个 lambda 表达式
    /// </summary>
    /// <param name="exp1"></param>
    /// <param name="condition">true 时生效</param>
    /// <param name="exp2"></param>
    /// <returns></returns>
    public static Expression<Func<T1, T2, T3, T4, T5, bool>> Or<T1, T2, T3, T4, T5>(this Expression<Func<T1, T2, T3, T4, T5, bool>> exp1, bool condition, Expression<Func<T1, T2, T3, T4, T5, bool>> exp2) => (Expression<Func<T1, T2, T3, T4, T5, bool>>)InternalAndOrExpression(condition, exp1, exp2, false);

    /// <summary>
    /// 将 lambda 表达式取反
    /// </summary>
    /// <param name="exp"></param>
    /// <param name="condition">true 时生效</param>
    /// <returns></returns>
    public static Expression<Func<T1, T2, T3, T4, T5, bool>> Not<T1, T2, T3, T4, T5>(this Expression<Func<T1, T2, T3, T4, T5, bool>> exp, bool condition = true) => (Expression<Func<T1, T2, T3, T4, T5, bool>>)InternalNotExpression(condition, exp);


    /// <summary>
    /// 组合继承属性选择表达式树,5个拓展参数 TResult将继承TBase的所有属性
    /// </summary>
    /// <typeparam name="TBase"> 原数据类型 </typeparam>
    /// <typeparam name="T1"> 拓展类型1 </typeparam>
    /// <typeparam name="T2"> 拓展类型2 </typeparam>
    /// <typeparam name="T3"> 拓展类型3 </typeparam>
    /// <typeparam name="T4"> 拓展类型4 </typeparam>
    /// <typeparam name="T5"> 拓展类型5 </typeparam>
    /// <typeparam name="TResult"> 返回类型 </typeparam>
    /// <param name="expression"> 拓展表达式 </param>
    /// <param name="exclude"> 排除字段 </param>
    /// <returns> </returns>
    public static Expression<Func<TBase, T1, T2, T3, T4, T5, TResult>> BuildExtendSelectExpre<TBase, T1, T2, T3, T4, T5, TResult>(this Expression<Func<TBase, T1, T2, T3, T4, T5, TResult>> expression, params string[] exclude)
    {
        return GetExtendSelectExpre<TBase, TResult, Func<TBase, T1, T2, T3, T4, T5, TResult>>(expression, exclude);
    }
    #endregion

    public static bool IsParameter(this Expression exp)
    {
        var test = new TestParameterExpressionVisitor();
        test.Visit(exp);
        return test.Result;
    }
    public static ParameterExpression GetParameter(this Expression exp)
    {
        var test = new GetParameterExpressionVisitor();
        test.Visit(exp);
        return test.Result;
    }

    static ConcurrentDictionary<Type, ConcurrentDictionary<string, MethodInfo>> _dicTypeMethod = new ConcurrentDictionary<Type, ConcurrentDictionary<string, MethodInfo>>();
    public static bool IsStringJoin(this MethodCallExpression exp, out Expression tolistObjectExpOut, out MethodInfo toListMethodOut, out LambdaExpression toListArgs0Out)
    {
        if (exp.Arguments.Count == 2 &&
            exp.Arguments[1].NodeType == ExpressionType.Call &&
            exp.Arguments[1].Type.FullName.StartsWith("System.Collections.Generic.List`1") &&
            exp.Arguments[1] is MethodCallExpression toListMethod &&
            toListMethod.Method.Name == "ToList" &&
            toListMethod.Arguments.Count == 1 &&
            toListMethod.Arguments[0] is UnaryExpression joinExpArgs1Args0Tmp &&
            joinExpArgs1Args0Tmp.Operand is LambdaExpression toListArgs0)
        {
            tolistObjectExpOut = toListMethod.Object;
            toListMethodOut = toListMethod.Type.GetGenericArguments().FirstOrDefault() == typeof(string) ?
                toListMethod.Method :
                toListMethod.Method.GetGenericMethodDefinition().MakeGenericMethod(typeof(string));
            toListArgs0Out = toListArgs0;
            return true;
        }
        tolistObjectExpOut = null;
        toListMethodOut = null;
        toListArgs0Out = null;
        return false;
    }

    public static object GetConstExprValue(this Expression exp)
    {
        if (exp.IsParameter()) return null;

        var expStack = new Stack<Expression>();
        var exp2 = exp;
        while (true)
        {
            switch (exp2?.NodeType)
            {
                case ExpressionType.Constant:
                    expStack.Push(exp2);
                    break;
                case ExpressionType.MemberAccess:
                    expStack.Push(exp2);
                    exp2 = (exp2 as MemberExpression).Expression;
                    if (exp2 == null) break;
                    continue;
                case ExpressionType.Call:
                    return Expression.Lambda(exp).Compile().DynamicInvoke();
                case ExpressionType.TypeAs:
                case ExpressionType.Convert:
                    var oper2 = (exp2 as UnaryExpression).Operand;
                    if (oper2.NodeType == ExpressionType.Parameter)
                    {
                        var oper2Parm = oper2 as ParameterExpression;
                        expStack.Push(exp2.Type.IsAbstract || exp2.Type.IsInterface ? oper2Parm : Expression.Parameter(exp2.Type, oper2Parm.Name));
                    }
                    else
                        expStack.Push(oper2);
                    break;
            }
            break;
        }
        object firstValue = null;
        switch (expStack.First().NodeType)
        {
            case ExpressionType.Constant:
                var expStackFirst = expStack.Pop() as ConstantExpression;
                firstValue = expStackFirst?.Value;
                break;
            case ExpressionType.MemberAccess:
                var expStackFirstMem = expStack.First() as MemberExpression;
                if (expStackFirstMem.Expression?.NodeType == ExpressionType.Constant)
                    firstValue = (expStackFirstMem.Expression as ConstantExpression)?.Value;
                else
                    return Expression.Lambda(exp).Compile().DynamicInvoke();
                break;
        }
        while (expStack.Any())
        {
            var expStackItem = expStack.Pop();
            switch (expStackItem.NodeType)
            {
                case ExpressionType.MemberAccess:
                    var memExp = expStackItem as MemberExpression;
                    if (memExp.Member.MemberType == MemberTypes.Property)
                        firstValue = ((PropertyInfo)memExp.Member).GetValue(firstValue, null);
                    else if (memExp.Member.MemberType == MemberTypes.Field)
                        firstValue = ((FieldInfo)memExp.Member).GetValue(firstValue);
                    break;
            }
        }
        return firstValue;
    }

    internal class TestParameterExpressionVisitor : ExpressionVisitor
    {
        public bool Result { get; private set; }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (!Result) Result = true;
            return node;
        }
    }

    internal class GetParameterExpressionVisitor : ExpressionVisitor
    {
        public ParameterExpression Result { get; private set; }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (Result == null) Result = node;
            return node;
        }
    }
    internal class NewExpressionVisitor : ExpressionVisitor
    {
        ParameterExpression[] _newParameters;
        ParameterExpression[] _oldParameters;
        public NewExpressionVisitor(ParameterExpression newParam, ParameterExpression oldParam) : this(new[] { newParam }, new[] { oldParam }) { }
        public NewExpressionVisitor(ParameterExpression[] newParams, ParameterExpression[] oldParams)
        {
            this._newParameters = newParams;
            this._oldParameters = oldParams;
        }
        public Expression Replace(Expression exp) => this.Visit(exp);

        protected override Expression VisitParameter(ParameterExpression node)
        {
            for (var a = 0; a < _oldParameters.Length; a++)
                if (_oldParameters[a] == node)
                    return _newParameters[a];
            return node;
        }
    }

    /// <summary>
    /// 继承ExpressionVisitor类，实现参数替换统一
    /// </summary>
    internal class ParameterReplaceVisitor : ExpressionVisitor
    {
        public ParameterReplaceVisitor(ParameterExpression paramExpr)
        {
            _parameter = paramExpr;
        }

        //新的表达式参数
        private readonly ParameterExpression _parameter;

        protected override Expression VisitParameter(ParameterExpression p)
        {
            return p.Type == _parameter.Type ? _parameter : (Expression)p;
        }
    }
}
