import { createRouter, createWebHistory } from "vue-router";
import SidebarMain from "@/components/layouts/SidebarMain.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "home",
      components: {
        default: () => import("@/views/HomeView.vue"),
        SideBar: SidebarMain,
      },
    },
  ],
});

export default router;
