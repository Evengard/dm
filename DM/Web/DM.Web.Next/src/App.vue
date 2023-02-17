<template>
  <div id="layout" :class="`theme-${theme}`">
    <header>
      <header-main></header-main>
    </header>
    <div id="sidebar">
      <RouterView name="SideBar" />
    </div>
    <main>
      <RouterView />
    </main>
    <footer></footer>
  </div>
</template>

<script setup lang="ts">
import { RouterView } from "vue-router";
import HeaderMain from "@/components/layouts/HeaderMain.vue";

const theme = "light";
</script>

<style lang="stylus">
@import url('https://fonts.googleapis.com/css2?family=PT+Sans:ital,wght@0,400;0,700;1,400;1,700&display=swap');
@import "assets/styles/themes.styl"

html, body, #app
  height 100%
  margin 0
  overflow hidden

body
  font-family 'PT Sans', sans-serif
  font-size 16px
  line-height 1.3
  word-wrap break-word

#layout
  display grid
  height 100%
  grid-template-columns 20rem 1fr
  grid-template-rows  3rem auto 2rem
  grid-template-areas "hd hd" "sd main" "ft ft"

header
  grid-area hd
  theme(background-color, $background)
  theme(color, $text)
  padding 0 1rem 0 2rem

footer
  grid-area ft
  theme(background-color, $backgroundReversed)
  theme(color, $textReversed)
  padding 0 1rem 0 2rem

#sidebar
  grid-area sd
  theme(background-color, $background)
  theme(color, $text)
  padding 0 1rem 0 2rem

main
  grid-area main
  theme(background-color, $background)
  theme(color, $text)
  theme(background-color, $background)

for num in (0..5)
  h{num+1}
    font-size (2.5rem - .25*num)
    margin 1rem 0 .5rem

a
  theme(color, $activeText)
  &:hover
    theme(color, $activeHoverText)
</style>
