<template>
  <h2><slot /></h2>
</template>

<script setup lang="ts"></script>

<style scoped lang="sass">
@import "src/assets/styles/Themes"

h2
  margin: $medium 0 $small
  font-size: $title-font-size
  +theme(color, $highlight-text)
</style>
