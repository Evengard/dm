<template>
  <div class="content">
    <div class="text">
      <h1><slot name="head" /></h1>
      <slot />
    </div>
    <div class="sidebar-right">
      <sidebar-rigth />
    </div>
  </div>
</template>

<script setup lang="ts">
import SidebarRigth from "@/components/layouts/SidebarRigth.vue";
</script>

<style scoped lang="stylus">
.content
  display grid
  grid-template-columns 1fr 15rem
  grid-column-gap 2rem
  @media screen and (max-width: 1600px)
    grid-template-columns 1fr 12rem
  @media screen and (max-width: 1200px)
    display block
</style>
