namespace Heatwave.Domain.System;
public class Resource : AuditableEntity
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 资源类型
    /// </summary>
    public ResourceType Type { get; set; }

    /// <summary>
    /// 连接类型
    /// </summary>
    public LinkType? LinkType { get; set; }

    /// <summary>
    /// 菜单为空，页面为连接，请求为权限值
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// 上级id
    /// </summary>
    public long? ParentId { get; set; }
}

/// <summary>
/// 资源类型
/// </summary>
public enum ResourceType
{
    /// <summary>
    /// 菜单
    /// </summary>
    Direction = 0,
    /// <summary>
    /// 页面
    /// </summary>
    Page = 1,
    /// <summary>
    /// 请求
    /// </summary>
    Action = 2,
}

/// <summary>
/// 资源链接类型
/// </summary>
public enum LinkType
{
    /// <summary>
    /// 内链
    /// </summary>
    Inner = 0,
    /// <summary>
    /// 外链
    /// </summary>
    Outer = 1,
}

