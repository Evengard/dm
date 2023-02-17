import { createApp } from "vue";
import { createPinia } from "pinia";

import App from "./App.vue";
import router from "./router";

import { library } from "@fortawesome/fontawesome-svg-core";
import { faDoorOpen, faBars } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
library.add(faDoorOpen, faBars);

import "@/assets/styles/refresh.css";

const app = createApp(App);

app.use(createPinia());
app.component("font-awesome-icon", FontAwesomeIcon);
app.use(router);

app.mount("#app");
