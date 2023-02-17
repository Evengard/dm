<template>
  <div class="nav">
    <router-link :to="{ name: 'home' }" class="logo">
      <img src="/img/logo.png" alt="DM.AM" />
    </router-link>
    <div class="menu-top">
      <a href="#">Правила</a>
    </div>
    <div class="profile">
      <a href="#">
        <font-awesome-icon icon="fa-solid fa-door-open" />
      </a>
    </div>
    <div class="menu-mobile">
      <font-awesome-icon
        icon="fa-solid fa-bars"
        @click="mobileListVisible = !mobileListVisible"
      />
      <div class="menu-mobile-list" :class="{ active: mobileListVisible }">
        <a href="#">Правила</a>
        <hr />
        <a href="#">Войти</a>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { RouterLink } from "vue-router";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import { ref } from "vue";

const mobileListVisible = ref(true);
</script>

<style scoped lang="stylus">
@import "../../assets/styles/themes.styl"

.nav
  display flex
  align-items center
  height 100%
  width 100%
  .logo
    width 22rem
    img
      max-height 2rem
  .menu-top
    flex 1
    font-size 1.25rem
  .profile
    text-align end
    margin-left 2rem
    font-size 1.5rem
  .menu-mobile
    display none
    text-align right
    margin-left 2rem
    flex 1
    font-size 1.5rem
    .menu-mobile-list
      display none
      position absolute
      right 1rem
      text-align left
      font-size 1rem
      theme(background-color, $background)
      padding .5rem
      border 1px solid
      theme(border-color, $border)
      &.active
        display flex
        flex-direction column
      & a
        padding .5rem
  @media screen and (max-width: 900px)
    .menu-top
    .profile
      display none
    .menu-mobile
      display block
</style>
