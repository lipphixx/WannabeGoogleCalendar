<script setup>
import {ref} from "vue";
import axios from "axios";

const email = ref(null);
const password = ref(null);

const emit = defineEmits(["fetchLogin"]);
const props = defineProps(['userId']);

async function fetchLogin() {
  const url = "https://172.20.10.4:5050/api/Auth/login";

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
    <div id="inputContainer">
      <label>
        Email:
        <input class="inputStyle" type="text" placeholder="Email" v-model="email">
      </label>
      <label>
        Heslo:
        <input class="inputStyle" type="password" placeholder="Heslo" v-model="password">
      </label>
    </div>
    <button type="submit">Přihlásit se</button>
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
  height: 10%;
  width: 300px;
  background-color: #36363650;
  border: 1px solid #36363690;
  color: White;
  border-radius: 5px;
}

button:hover {
  background-color: #36363680;
  -webkit-box-shadow: inset 0px 0px 0px 3px rgba(81, 81, 81, 0.30);
}
</style>