<template>
  <div class="nav">
    <router-link :to="{ name: 'home' }" class="logo">
      <img src="/img/logo.png" alt="DM.AM" />
    </router-link>
  </div>
</template>

<script setup lang="ts">
import { RouterLink } from "vue-router";
</script>

<style scoped lang="stylus">
.nav
  display flex
  justify-content space-between
  align-items center
  height 100%
  .logo
    margin-right 3rem
    img
      max-height 2rem
</style>
