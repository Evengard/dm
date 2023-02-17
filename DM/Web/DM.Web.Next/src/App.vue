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
// const theme = "dark";
</script>

<style lang="stylus">
@import url('https://fonts.googleapis.com/css2?family=PT+Sans:ital,wght@0,400;0,700;1,400;1,700&display=swap');
@import "assets/styles/themes.styl"

html, body, #app
  height 100%
  margin 0

body
  font-family 'PT Sans', sans-serif
  font-size 16px
  line-height 1.25
  word-wrap break-word

#layout
  display grid
  height 100%
  grid-template-columns 20rem 1fr
  grid-template-rows  3rem auto 2rem
  grid-template-areas "hd hd" "sd main" "ft ft"
  @media screen and (max-width: 1200px)
    grid-template-columns 15rem 1fr
  @media screen and (max-width: 900px)
    display block
    header
      height 3rem
      padding 0 1rem
    footer
      height 2rem
      padding 0 1rem
    main
      padding 0 1rem

header
  grid-area hd
  theme(background-color, $panelBackground)
  theme(color, $textHeading)
  padding 0 2rem
  a
    theme(color, $textHeading)
    &:hover
      text-decoration none

footer
  grid-area ft
  theme(background-color, $panelBackground)
  theme(color, $textReversed)
  padding 0 2rem

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
  padding 0 2rem 2rem 0

for num in (0..5)
  h{num+1}
    font-size (1.75rem - .125*num)
    margin 1rem 0 .5rem
    theme(color, $textHeading)

p
  font-size 1.25rem
  margin-bottom .75rem

a
  theme(color, $activeText)
  &:hover
    theme(color, $activeHoverText)
    text-decoration underline

b
strong
  font-weight bold

em
i
  font-style italic

ul
ol
  margin 0 0 .5rem .5rem
  font-size 1.125rem
  padding-left 1rem
  li
    margin .25rem .5rem

ul
  li
    list-style disc outside

ol
  li
    list-style decimal outside
    padding-left .25rem

hr
  border-top: 1px solid
  theme(border-top-color, $textHeading)
</style>
