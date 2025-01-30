<script setup>

import Calendar from "@/components/calendar.vue";
import registerPage from "@/components/registerPage.vue";
import loginPage from "@/components/loginPage.vue";
import {computed, ref} from "vue";

const register = document.querySelector('a#register');
const login = document.querySelector('a#login');

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

function hideMenu() {
  register.style.display = "none";
  login.style.display = "none";
}

</script>

<template>
  <section>
    <a href="#/registerPage" id="register" @click="hideMenu">Registrovat se</a>
    <a href="#/loginPage" id="login" @click="hideMenu">Přihlásit se</a>
  </section>

  <component :is="currView"></component>

</template>

<style scoped>

</style>
