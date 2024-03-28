namespace Heatwave.Domain.System;

/// <summary>
/// 资源数据
/// </summary>
[Table("sys_resource")]
public class Resource : AuditableEntity
{
    /// <summary>
    /// 程序资源id
    /// </summary>
    public long AppId { get; set; }
    
    /// <summary>
    /// 资源类型
    /// </summary>
    public ResourceType Type { get; set; }

    /// <summary>
    /// 上级菜单
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 前端组件名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 前端路由地址(views起步)
    /// </summary>
    /// <remarks>
    /// path随便写，但前面必须有个 `/`
    /// </remarks>
    public string? Path { get; set; }

    /// <summary>
    /// 前端展示组件
    /// </summary>
    /// <remarks>
    /// component对应的值前不需要加 / 值对应的是实际业务 `.vue` 或 `.tsx` 代码路径
    /// </remarks>
    public string? Component { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Rank { get; set; }

    // public string Redirect { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; }


    /// <summary>
    /// 是否缓存
    /// </summary>
    public bool KeepAlive { get; set; }

    /// <summary>
    /// 权限值
    /// </summary>
    public string? PermissionCode { get; set; }

    /// <summary>
    /// 是否在页面显示
    /// </summary>
    public bool ShowLink { get; set; }

    /// <summary>
    /// 是否显示父级
    /// </summary>
    public bool ShowParent { get; set; }
}

/// <summary>
/// 资源类型
/// </summary>
public enum ResourceType
{
    /// <summary>
    /// 目录
    /// </summary>
    Catalogue = 0,
    /// <summary>
    /// 菜单
    /// </summary>
    IFrame = 1,

    /// <summary>
    /// 外链
    /// </summary>
    OutLink = 2,
    /// <summary>
    /// 请求
    /// </summary>
    Request = 3,
}