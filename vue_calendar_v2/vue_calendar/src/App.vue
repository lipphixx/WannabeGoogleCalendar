<script setup>
import {computed, onMounted, reactive, ref, watch} from "vue";
import Calendar from "@/components/calendar.vue";
import registerPage from "@/components/registerPage.vue";
import loginPage from "@/components/loginPage.vue";
import AppHeader from "@/components/AppHeader.vue";

const onLogin = ref(true); // Stav pro přihlášení
const currPath = ref(window.location.hash);
const userId = ref(null);
const username = ref(null);
const refAction = ref({});


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
    userId.value = user.userId;
    username.value = user.username;
    console.log(userId.value, username.value);
  }
}
function logOut(){
  userId.value = null;
  window.location.hash = '#';
}

</script>

<template>
  <main>
    <div id="appHeader">
    <AppHeader :logged-user="username" v-if="onLogin"
               @log-out="logOut"
               :action="refAction"></AppHeader>
    </div>

  <section v-if="!onLogin">
    <a href="#/registerPage">Registrovat se</a>
    <a href="#/loginPage">Přihlásit se</a>
  </section>

  <component :is="currView" @fetchLogin="fetchLogin"></component>

    <div id="calendarComp">
  <Calendar v-if="onLogin" :loggedUser="userId"
            :action="refAction" @update:action="refAction = $event"/>
    </div>
  </main>
</template>

<style scoped>


main {
  display: flex;
  flex-direction: column;
  margin: 0;
}

#appHeader {
display: flex;
  width: 100%;
}

#calendarComp {
  display: flex;
  width: 90%;
  justify-content: center;
  text-align: center;
 }
</style>