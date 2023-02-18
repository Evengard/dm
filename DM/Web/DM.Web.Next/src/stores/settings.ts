import { ref } from "vue";
import { defineStore } from "pinia";

export const useSettingsStore = defineStore("settings", () => {
  const theme = ref("");
  function getTheme() {
    const localTheme = localStorage.getItem("settingsTheme") as string;
    if (localTheme) {
      // Если есть сохранённое, то получаем из хранилища
      theme.value = localTheme;
    } else {
      // Иначе пытаемся взять из темы браузера
      const prefersDarkScheme = window.matchMedia(
        "(prefers-color-scheme: dark)"
      );
      if (prefersDarkScheme.matches) {
        theme.value = "dark";
      } else {
        theme.value = "light";
      }
      localStorage.setItem("settingsTheme", theme.value);
    }
  }
  function setTheme() {
    if (theme.value == "light") {
      theme.value = "dark";
    } else {
      theme.value = "light";
    }
    localStorage.setItem("settingsTheme", theme.value);
  }

  return { theme, getTheme, setTheme };
});
