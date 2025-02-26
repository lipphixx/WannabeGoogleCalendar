<script setup>
import {computed, onMounted, ref} from "vue";
import Calendar from "@/components/calendar.vue";
import AppHeader from "@/components/AppHeader.vue";
import AppSidebar from "@/components/AppSidebar.vue";
import LoginPage from "@/components/loginPage.vue";
import RegisterPage from "@/components/registerPage.vue";
import PasswordReset from "@/components/passwordReset.vue";
import CakeBot from "@/components/cakeBot.vue";

const onLogin = ref(false);
const currPath = ref("#/loginPage");
const userId = ref(null);
const username = ref(null);
const refAction = ref({});
const isActive = ref(false);
const currentMonth = ref(0);
const currentYear = ref(0);

const showEvents = ref(true);
const showHolidays = ref(true);
const showModal = ref({});
const chatbotToggle = ref(false);

const onLoading = ref(false);

const routes = {
  "/registerPage": RegisterPage,
  "/loginPage": LoginPage,
};

const labelColors = {
  red: { color: '#FF0000', emoji: '🔴' },
  blue: { color: '#0000FF', emoji: '🔵' },
  green: { color: '#00FF00', emoji: '🟢' },
  orange: { color: '#FF8000', emoji: '🟠' },
  purple: { color: '#8000FF', emoji: '🟣' }
};

onMounted(() => {
  const date = new Date();
  currentMonth.value = date.getMonth();
  currentYear.value = date.getFullYear();

  if (!currPath.value || currPath.value === "#") {
    window.location.hash = "#/loginPage";
    currPath.value = "#/loginPage";
  }
})


const isRegisterPage = computed(() => currPath.value.replace("#", "") === "/registerPage");

window.addEventListener("hashchange", () => {
  currPath.value = window.location.hash;
});

window.addEventListener("load", () => {
  if (sessionStorage.getItem('loggedUser') && sessionStorage.getItem('loggedUsername')) {
    userId.value = sessionStorage.getItem('loggedUser');
    username.value = sessionStorage.getItem('loggedUsername');
    const user = {
      userId: userId.value,
      username: username.value
    }
    fetchLogin(true, user);
  } else {
    onLogin.value = false;
    window.location.hash = '#';
    userId.value = null;
    username.value = null;
  }
});

const currView = computed(() => {
  return routes[currPath.value.slice(1)] || LoginPage;
});


function fetchLogin(loginPhase, user) {
  onLogin.value = loginPhase;
  if (loginPhase) {
    window.location.hash = '#/calendar';
    userId.value = user.userId;
    username.value = user.username;
  }
}

function logOut() {
  userId.value = null;
  username.value = null;
  window.location.hash = '#';
  sessionStorage.removeItem('loggedUser');
  sessionStorage.removeItem('loggedUsername');
}

function toggleEvents(state) {
  showEvents.value = !state;
}

function toggleHolidays(state) {
  showHolidays.value = !state;
}

function loading() {
  onLoading.value = !onLoading.value;
}
</script>

<template>
  <body>
  <main>
    <div id="appHeader">
      <AppHeader v-if="onLogin"
                 @openMenu="isActive = !isActive"
                 :action="refAction"
                 :currentMonth="currentMonth"
                 :currentYear="currentYear"></AppHeader>
    </div>

    <section v-if="!onLogin" class="buttony">
      <p v-if="isRegisterPage">Máte již účet? <a href="#/loginPage">Přihlaste se</a></p>
      <p v-if="!isRegisterPage">Nemáte vytvořený účet? <a href="#/registerPage">Zaregistrujte se</a></p>
    </section>

    <component :is="currView" @fetchLogin="fetchLogin" id="loginOrRegister"
               @onLoadin="loading"></component>
    <div class="calendarComp" :class="{ calendarActive: isActive, onLoad : onLoading}">
      <Calendar v-if="onLogin" :loggedUser="userId"
                :action="refAction" @update:action="refAction = $event"
                :loggedUsername="username"
                :showEvents="showEvents"
                :showHolidays="showHolidays"
                :createEvent="showModal" @update:createEvent="showModal = $event"
                :labelColors="labelColors"
                @onLoading="loading"
      />
    </div>
    <p v-if="onLoading" id="fixed" style="font-weight: bolder">Loading...</p>
    <AppSidebar class="sidebar" :class="{ sidebarActive: isActive }"
                :loggedUser="username" v-if="onLogin"
                :action="refAction"
                :currentMonth="currentMonth"
                :currentYear="currentYear"
                :createEvent="showModal"
                :labelColors="labelColors"
                @chatbotToggle="chatbotToggle = true"
                @logOut="logOut"
                @userEvents="toggleEvents"
                @holidayEvents="toggleHolidays"
    ></AppSidebar>

  </main>
  <div id="bot">
    <cakeBot :chatbotToggle="chatbotToggle" @update:chatbotToggle="chatbotToggle = $event"></cakeBot>
  </div>
  </body>
</template>

<style scoped>
#bot {
  width: 200px;
  position: fixed;
}

body, main, .calendarComp {
  background-color: #181818FF;
}

.onLoad {
  filter: blur(5px);
}

main {
  display: flex;
  flex-direction: column;
  margin: 0;
}

section {
  display: flex;
  gap: 5px;
}

#fixed {
  position: fixed;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
}

section p {
  height: 10%;
  width: 300px;
  color: White;
  font-size: small;
  display: flex;
  gap: 5px;
  text-align: center;
  justify-content: center;
  align-items: center;

}

a {
  text-decoration: none;
  color: cornflowerblue;
}

a:hover {
  text-decoration: underline;
}

.buttony {
  position: absolute;
  top: 80%;
  left: 50%;
  transform: translate(-50%, -50%);
}

#appHeader {
  display: flex;
  width: 100%;
}

.calendarComp {
  display: flex;
  position: fixed;
  top: 7vh;
  left: 0;
  width: 100vw;

  transition: filter .2s ease-in, left 1s ease-in-out;
}

.calendarActive {
  left: -7vw;
}

.sidebar {
  position: fixed;
  right: -15%;
  height: 100vh;
  top: 6.5%;
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
  margin-top: 5%;
}


</style>