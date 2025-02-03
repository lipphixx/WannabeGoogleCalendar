<script setup>
import { ref } from "vue";
import axios from "axios";

const email = ref(null);
const password = ref(null);

const emit = defineEmits(["fetchLogin"]);
const props = defineProps(['userId']);

async function fetchLogin() {
  const url = "https://localhost:7198/api/Auth/login";

  try {
    const loginDetails = {
      email: email.value,
      password: password.value,
      twoFactorCode: "1234",
      twoFactorRecoveryCode: "abcd",
    };

    const response = await axios.post(url, loginDetails);
    console.log(response);
    emit("fetchLogin", true, response.data); // Odeslat event do rodiče
    console.log(response.data);
  } catch (error) {
    console.log(error);
    emit("fetchLogin", false);
  }
}
</script>

<template>
  <form @submit.prevent="fetchLogin">
    <h2>Login</h2>
    <label>
      Email:
      <input type="text" placeholder="Jméno" v-model="email">
    </label>
    <label>
      Heslo:
      <input type="password" placeholder="Příjmení" v-model="password">
    </label>
    <button type="submit">Přihlásit se</button>
  </form>
</template>
