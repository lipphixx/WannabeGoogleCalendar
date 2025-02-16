<script setup>

import {ref} from "vue";
import axios from "axios";

const name = ref(null);
const surname = ref(null);
const username = ref(null);
const email = ref(null);
const phone = ref(null);
const password = ref(null);

const emit = defineEmits('fetchLogin');

async function postRegister() {
  const url = "https://localhost:5050/api/Auth/register";

  const fullName = `${name.value} ${surname.value}`
  const registerDetails = {
    email: email.value,
    password: password.value,
    username: username.value,
    fullName: fullName,
    phoneNumber: phone.value.toString()
  };

  try {
    const response = await axios.post(url, registerDetails);
    emit("fetchLogin", true, response.data);
  } catch (error) {
    console.log(error);
    emit("fetchLogin", false);
  }

}
</script>

<template>
  <form @submit.prevent="postRegister">
    <h2>Registrace</h2>
    <div id="inputContainer">
      <label>
        Jméno:
        <input type="text" placeholder="Jméno" v-model="name" autocomplete="off">
      </label>
      <label>
        Příjmení:
        <input type="text" placeholder="Příjmení" v-model="surname" autocomplete="off">
      </label>
      <label>
        Uživatelské jméno:
        <input type="text" placeholder="Username" v-model="username" autocomplete="off">
      </label>
      <label>
        E-mail:
        <input type="text" placeholder="Email" v-model="email" autocomplete="off">
      </label>
      <label>
        Telefonní číslo:
        <input type="number" placeholder="Tel Aviv" v-model="phone" autocomplete="off">
      </label>
      <label>
        Heslo:
        <input type="password" placeholder="Heslo" v-model="password" autocomplete="off">
      </label>
    </div>
    <button type="submit">Registrovat se</button>
  </form>
</template>

<style scoped>

form {
  display: flex;
  width: 100%;
}

label {
  display: flex;
  flex-direction: column;
}

#inputContainer {
  margin-bottom: 10px;
  display: flex;
  flex-direction: column;
}

input {
  width: 300px;
  padding: 8px;
  margin: 5px 0;
  border-radius: 4px;
  border: 1px solid #ddd;
}

button {
  flex-shrink: 0;
  height: 10%;
  width: 300px;
  background: linear-gradient(to right, cornflowerblue, #0d5be8) left;
  background-size: 200% 100%;
  transition: background-position 0.3s ease-in-out;
  color: White;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

button:hover {
  background-position: right;
}

</style>