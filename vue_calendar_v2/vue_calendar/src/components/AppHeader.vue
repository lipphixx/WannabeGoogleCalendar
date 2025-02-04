<script setup>

import {ref} from "vue";

const props = defineProps(['loggedUser']);
const menuDialog = ref(null);
const emit = defineEmits(['logOut']);

function showMenuDialog() {
  menuDialog.value.showModal();
}
</script>

<template>
  <head>

    <title>sdafsydf</title>
    <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:opsz,wght,FILL,GRAD@24,400,0,0&icon_names=menu" />
  </head>

  <header>

    <a class="material-symbols-outlined" @click.prevent="showMenuDialog"> menu </a>
  </header>

  <dialog ref="menuDialog">
    <div id="firRow">
      <!--        <img src="vue_calendar/src/assets/user.svg" alt="user">-->
      <p>{{props.loggedUser}}</p>
    </div>
    <div id="secRow">
      <!--        <img src="vue_calendar/src/assets/settings.svg" alt="settings">-->
      <div>
        <!--          <img src="vue_calendar/src/assets/logout.svg">-->
        <button @click="emit('logOut')">Log out</button>
      </div>
    </div>
  </dialog>
</template>

<style scoped>
header {
  height: 50px;
}
</style>