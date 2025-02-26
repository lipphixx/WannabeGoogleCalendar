<script setup>
import { onMounted, ref } from "vue";
import axios from "axios";

const name = ref(null);
const surname = ref(null);
const username = ref(null);
const email = ref(null);
const phone = ref(null);
const password = ref(null);
const errorValue = ref('');
const emit = defineEmits(['fetchLogin']);

onMounted(() => {
  errorValue.value = '';
});

async function postRegister() {
  const url = "https://172.20.10.4:5050/api/Auth/register";
  if (!name.value || !surname.value || !phone.value || !username.value || !password.value) {
    errorValue.value = 'invalid';
    return;
  }

  const fullName = `${name.value} ${surname.value}`;
  const registerDetails = {
    email: email.value,
    password: password.value,
    username: username.value,
    fullName: fullName,
    phoneNumber: phone.value.toString()
  };

  try {
    const response = await axios.post(url, registerDetails);
    errorValue.value = '';
    emit("fetchLogin", true, response.data);
  } catch (error) {
    console.log('Chyba:', error.response?.status);
    if (error.response) {
      switch (error.response.status) {
        case 400:
          errorValue.value = 'mail';
          break;
        case 404:
          errorValue.value = 'phone';
          break;
        case 401:
          errorValue.value = 'username';
          break;
        default:
          errorValue.value = 'network';
      }
    } else {
      errorValue.value = 'network';
    }
    emit("fetchLogin", false);
  }
}
</script>

<template>
  <form @submit.prevent="postRegister">
    <h2>Registrace</h2>
    <div id="inputContainer">
      <div style="display: flex; gap: 1vw">
        <label> Jméno:
          <input type="text" placeholder="Jméno" v-model="name" autocomplete="off">
        </label>
        <label> Příjmení:
          <input type="text" placeholder="Příjmení" v-model="surname" autocomplete="off">
        </label>
      </div>
      <label> Uživatelské jméno:
        <input type="text" placeholder="Uživatelské jméno" v-model="username" autocomplete="off">
      </label>
      <p v-if="errorValue === 'username'" style="color: red">Účet s tímto uživatelským jménem již existuje.</p>
      <label> E-mail:
        <input type="text" placeholder="Emailová adresa" v-model="email" autocomplete="off">
      </label>
      <p v-if="errorValue === 'mail'" style="color: red">Účet s tímto e-mailem již existuje.</p>
      <label> Telefonní číslo:
        <input type="tel" placeholder="Telefonní číslo" v-model="phone" autocomplete="off">
      </label>
      <p v-if="errorValue === 'phone'" style="color: red">Účet s tímto telefonním číslem již existuje.</p>
      <label> Heslo:
        <input type="password" placeholder="Heslo" v-model="password" autocomplete="off">
      </label>
      <p v-if="errorValue === 'invalid'" style="color: red">Pole nesmí být prázdná.</p>
      <button type="submit">Registrovat se</button>
    </div>
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
  width: 300px;
}

input {
  width: 100%;
  padding: 8px;
  margin: 5px 0;
  border-radius: 4px;
  border: 1px solid #ddd;
}

button {
  flex-shrink: 0;
  height: 10%;
  width: 100%;
  background: linear-gradient(to right, cornflowerblue, #0d5be8) left;
  background-size: 200% 100%;
  transition: background-position 0.3s ease-in-out;
  color: White;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  margin-top: 2vh;
}

button:hover {
  background-position: right;
}

</style>