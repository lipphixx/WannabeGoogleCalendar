<script setup>
import { computed, ref } from "vue";
import Calendar from "@/components/calendar.vue";
import registerPage from "@/components/registerPage.vue";
import loginPage from "@/components/loginPage.vue";
import AppHeader from "@/components/AppHeader.vue";

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

function fetchLogin(loginPhase, user) {
  onLogin.value = loginPhase;
  console.log(onLogin.value)
  if (loginPhase) {
    window.location.hash = '#/calendar';
    userId.value = user;
    console.log(userId.value)
  }
}

function logOut(){
  userId.value = null;
  window.location.hash = '#';

}

</script>

<template>
  <section v-if="!onLogin">
    <a href="#/registerPage">Registrovat se</a>
    <a href="#/loginPage">Přihlásit se</a>
  </section>

  <component :is="currView" @fetchLogin="fetchLogin"></component>

  <Calendar v-if="onLogin" :loggedUser="userId"/>

  <AppHeader :logged-user="userId" v-if="onLogin"
  @log-out="logOut"></AppHeader>
</template>

