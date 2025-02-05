<script setup>

import {ref} from "vue";

const props = defineProps(['loggedUser', 'action']);
const menuDialog = ref(null);
const emit = defineEmits(['logOut']);

function showMenuDialog() {
  menuDialog.value.showModal();
}

function logOut() {
  emit('logOut');
  window.location.reload();
}
</script>

<template>
  <head>
    <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:opsz,wght,FILL,GRAD@24,400,0,0"/>
  </head>

  <header>
    <div class="tlacitka">
      <button @click="props.action.action = 1">Předchozí</button>
      <button @click="props.action.action = 2">Následující</button>
      <button @click="props.action.action = 3">Nyní</button>
    </div>
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
  justify-content: center;
  background-color: white;
  width: 100%;
}

#firRow {
  display: flex;
  align-items: center;
  margin-left: -5px;
}

.tlacitka {
  display: flex;
}
</style>