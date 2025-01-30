<script setup>

import Calendar from "@/components/calendar.vue";
import registerPage from "@/components/registerPage.vue";
import loginPage from "@/components/loginPage.vue";
import {computed, onMounted, ref} from "vue";

const register = document.querySelector('a#register');
const login = document.querySelector('a#login');

const onLogin = ref(true);

const routes = {
  '/registerPage': registerPage,
  '/loginPage': loginPage
}

const currPath = ref(window.location.hash);

window.addEventListener('hashchange', () => {
  currPath.value = window.location.hash;
});

const currView = computed(() => {
  const newRoute= currPath.value.slice(1);

  if (!newRoute) {
    return null;
  }

  return routes[newRoute];
});

function hideMenu(loginPhase) {
  onLogin.value = loginPhase;
}

</script>

<template>
  <section v-if="onLogin">
    <a href="#/registerPage" id="register">Registrovat se</a>
    <a href="#/loginPage" id="login">Přihlásit se</a>
  </section>

  <component :is="currView"
  @fetchLogin="hideMenu"></component>

</template>

<style scoped>

</style>
