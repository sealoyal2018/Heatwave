const Layout = () => import("@/layout/index.vue");

export default {
  path: "/",
  name: "system",
  component: Layout,
  redirect: "/resource",
  meta: {
    icon: "ep:home-filled",
    title: "系统管理",
    showLink: true,
    rank: 0
  },
  children: [
    {
      path: "/resource/list",
      name: "resourcelist",
      component: () => import("@/views/resource/list.vue"),
      meta: {
        title: "权限管理",
        showLink: true,
        showParent: true,
      }
    }
  ]
} satisfies RouteConfigsTable;
