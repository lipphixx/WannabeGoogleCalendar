<script setup>

import {ref} from "vue";

const name = ref(null);
const surname = ref(null);
const username = ref(null);
const email = ref(null);
const phone = ref(0);
const password = ref(null);
</script>

<template>
  <div>
    <h2>Registrace</h2>
    <label>
      Jméno:
      <input type="text" placeholder="Jméno" v-model="name">
    </label>
    <label>
      Příjmení:
      <input type="text" placeholder="Příjmení" v-model="surname">
    </label>
    <label>
      Uživatelské jméno:
      <input type="text" placeholder="Username" v-model="username">
    </label>
    <label>
      E-mail:
      <input type="text" placeholder="Email" v-model="email">
    </label>
    <label>
      Telefonní číslo:
      <input type="number" placeholder="Tel Aviv" v-model="phone">
    </label>
    <label>
      Heslo:
      <input type="password" placeholder="Heslo" v-model="password">
    </label>
    <button>Registrovat se</button>
  </div>
</template>

<style scoped>

</style>