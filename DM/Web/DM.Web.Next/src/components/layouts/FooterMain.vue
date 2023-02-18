<template>
  <div class="footer">
    <div class="copy">&copy; DM.AM</div>
    <div class="settings">
      <font-awesome-icon
        icon="fa-solid fa-lightbulb"
        @click="settings.setTheme()"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { useSettingsStore } from "@/stores/settings";

const settings = useSettingsStore();
</script>

<style scoped lang="stylus">
.footer
  display flex
  align-items center
  height 100%
  width 100%
  .copy
    flex 1
  .settings
    text-align right
    margin-left 2rem
    & > *
      cursor pointer
</style>
