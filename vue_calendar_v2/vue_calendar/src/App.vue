<script setup>
import { computed, ref } from "vue";
import Calendar from "@/components/calendar.vue";
import registerPage from "@/components/registerPage.vue";
import loginPage from "@/components/loginPage.vue";

const onLogin = ref(false); // Stav pro přihlášení
const currPath = ref(window.location.hash);
const userId = ref(null);

const routes = {
  "/registerPage": registerPage,
  "/loginPage": loginPage
};

window.addEventListener("hashchange", () => {
  currPath.value = window.location.hash;
});

const currView = computed(() => {
  const newRoute = currPath.value.slice(1);
  if (!newRoute) return null;
  return routes[newRoute];
});

function hideMenu(loginPhase, user) {
  onLogin.value = loginPhase;
  console.log(onLogin.value)
  if (loginPhase) {
    window.location.hash = '#/calendar';
    userId.value = user;
    console.log(userId.value)
  }
}
</script>

<template>
  <section v-if="!onLogin">
    <a href="#/registerPage">Registrovat se</a>
    <a href="#/loginPage">Přihlásit se</a>
  </section>

  <component :is="currView" @fetchLogin="hideMenu"></component>

  <Calendar v-if="onLogin" :loggedUser="userId" />
</template>

