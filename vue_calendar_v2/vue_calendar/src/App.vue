<script setup>
import {computed, onMounted, reactive, ref, watch} from "vue";
import Calendar from "@/components/calendar.vue";
import registerPage from "@/components/registerPage.vue";
import loginPage from "@/components/loginPage.vue";
import AppHeader from "@/components/AppHeader.vue";
import AppSidebar from "@/components/AppSidebar.vue";

const onLogin = ref(false); // Stav pro přihlášení
const currPath = ref(window.location.hash);
const userId = ref(null);
const username = ref(null);
const refAction = ref({});
const sidebar = document.querySelector('div#sidebar');
const isActive = ref(false);

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

function logOut() {
  userId.value = null;
  window.location.hash = '#';
}


</script>

<template>
  <main>
    <div id="appHeader">
      <AppHeader v-if="onLogin"
                 @openMenu="isActive = !isActive"
      :action="refAction"></AppHeader>
    </div>

    <section v-if="!onLogin">
      <a href="#/registerPage">Registrovat se</a>
      <a href="#/loginPage">Přihlásit se</a>
    </section>

    <component :is="currView" @fetchLogin="fetchLogin" id="loginOrRegister"></component>
    <div class="calendarComp" :class="{ calendarActive: isActive}">
      <Calendar v-if="onLogin" :loggedUser="userId"
                :action="refAction" @update:action="refAction = $event"
      :loggedUsername="username"/>
    </div>
      <AppSidebar class="sidebar" :class="{ sidebarActive: isActive }"
                  :loggedUser="username" v-if="onLogin"
                  :action="refAction"
      @logOut="logOut"></AppSidebar>
  </main>
</template>

<style scoped>


main {
  display: flex;
  flex-direction: column;
  margin: 0;
}

section {
  display: flex;
  gap: 5px;
}

section a {
  height: 10%;
  width: 150px;
  text-align: center;
  background-color: #36363650;
  border: 1px solid #36363690;
  color: White;
  border-radius: 5px;

}

#appHeader {
  display: flex;
  width: 100%;
}

.calendarComp {
  display: flex;
  position: fixed;
  top: 8%;
  left: 0;
  transition: left 1s ease-in-out;
}

.calendarActive {
  left: -7%;
}

.sidebar {
  position: fixed;
  right: -15%;
  height: 100vh;
  top: 7%;
  transition: right 1s ease-in-out;
}

.sidebarActive {
  right: 0;
}

#loginOrRegister {
  display: flex;
  flex-direction: column;
  align-items: center; /* Zarovná vodorovně */
  height: 50vh; /* Zajistí vertikální zarovnání */
  width: 100%;
}


</style>