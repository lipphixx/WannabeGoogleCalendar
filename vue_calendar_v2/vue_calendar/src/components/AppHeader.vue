<script setup>

import {onMounted, ref} from "vue";

const props = defineProps(['loggedUser', 'action']);
const menuDialog = ref(null);
const emit = defineEmits(['logOut']);
const currentMonth = ref(0);
const currentYear = ref(0);

onMounted(() => {
  const date = new Date();
  currentMonth.value = date.getMonth();
  currentYear.value = date.getFullYear();
});
function showMenuDialog() {
  menuDialog.value.showModal();
}

function logOut() {
  emit('logOut');
  window.location.reload();
}

function nextMonth() {
  props.action.action = 0;
  props.action.action = 2;
  if (currentMonth.value === 11) {
    currentMonth.value = 0;
    currentYear.value++;
  } else {
    currentMonth.value++;
  }
}

function previousMonth() {
  props.action.action = 0;
  props.action.action = 1;
  if (currentMonth.value === 0) {
    currentMonth.value = 11;
    currentYear.value--;
  } else {
    currentMonth.value--;
  }
}

function showNow() {
  props.action.action = 0;
  props.action.action = 3;
  const now = new Date();
  currentMonth.value = now.getMonth();
  currentYear.value = now.getFullYear();
}
</script>

<template>
  <head>
    <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:opsz,wght,FILL,GRAD@24,400,0,0"/>
  </head>

  <header>
    <div class="tlacitka">
      <button class="tlacitko" @click="previousMonth">Předchozí</button>
      <button class="tlacitko" @click="nextMonth">Následující</button>
      <button class="tlacitko" @click="showNow">Nyní</button>
    </div>

    <h1>{{new Date(currentMonth, currentMonth).toLocaleDateString('cs-CZ', { month: 'long' })}} {{currentYear}}</h1>

    <a class="material-symbols-outlined" @click.prevent="showMenuDialog"> menu </a>

  </header>

  <dialog ref="menuDialog">
    <div id="firRow">
      <a class="material-symbols-outlined" @click.prevent="showMenuDialog"> person </a>
      <p>{{props.loggedUser}}</p>
    </div>
    <div id="secRow">
      <!--        <img src="vue_calendar/src/assets/settings.svg" alt="settings">-->
      <div>
        <!--          <img src="vue_calendar/src/assets/logout.svg">-->
        <button @click="logOut">Log out</button>
      </div>
    </div>
  </dialog>
</template>

<style scoped>
header {
  height: 50px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  background-color: #1f1f1f;
  width: 100%;
  padding-left: 2%;
  padding-right: 2%;
}

h1 {
  margin-right: 11%;
  text-align: center;
  color: rgba(255, 255, 255, 0.9);
}

#firRow {
  display: flex;
  align-items: center;
  margin-left: -5px;
}

.tlacitka {
  display: flex;
  gap: 5px;
}

.tlacitko {
  background-color: #36363650;
  border: 1px solid #36363690;
  padding: 5%;
  color: White;
  border-radius: 5px;
}

.tlacitko:hover {
  background-color: #36363680;
  -webkit-box-shadow:inset 0px 0px 0px 3px rgba(81, 81, 81, 0.30);
}
</style>