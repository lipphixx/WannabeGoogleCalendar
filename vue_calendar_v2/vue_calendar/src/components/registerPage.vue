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
  <form>
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
  </form>
</template>

<style scoped>
.login {
  width: 300px;
  margin: auto;
  padding: 20px;
  border: 1px solid #ddd;
  border-radius: 5px;
}

form div {
  margin-bottom: 10px;
}

input {
  width: 100%;
  padding: 8px;
  margin: 5px 0;
  border-radius: 4px;
  border: 1px solid #ddd;
}

button {
  width: 100%;
  padding: 10px;
  background-color: #4caf50;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

button:hover {
  background-color: #45a049;
}

.error {
  color: red;
  font-size: 14px;
  margin-top: 10px;
}
</style>