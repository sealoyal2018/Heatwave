using System.ComponentModel.DataAnnotations.Schema;

namespace Heatwave.Domain.System;

/// <summary>
/// 资源数据
/// </summary>
[Table("sys_resource")]
public class Resource : AuditableEntity
{
    /// <summary>
    /// 名称
    /// </summary>
    public string MenuName { get; set; }

    /// <summary>
    /// 资源类型
    /// </summary>
    public ResourceType MenuType { get; set; }

    /// <summary>
    /// 状态true-正常，false-停用
    /// </summary>
    public bool State { get; set; }

    /// <summary>
    /// 权限值
    /// </summary>
    public string PermissionCode { get; set; }

    /// <summary>
    /// 上级id
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 菜单图标
    /// </summary>
    public string MenuIcon { get; set; }

    /// <summary>
    /// 路由地址
    /// </summary>
    public string Router { get; set; }

    /// <summary>
    /// 是否为链接
    /// </summary>
    public bool IsLink { get; set; }

    /// <summary>
    /// 是否缓存
    /// </summary>
    public bool IsCache { get; set; }

    /// <summary>
    /// 是否显示
    /// </summary>
    public bool IsShow { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// 前端展示组件
    /// </summary>
    public string Component { get; set; }

    /// <summary>
    /// 路由参数
    /// </summary>
    public string Query { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int OrderNum { get; set; }
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
    Menu = 1,
    /// <summary>
    /// 请求
    /// </summary>
    Action = 2,
}