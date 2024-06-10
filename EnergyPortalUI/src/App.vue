<template>
  <div class="app-container">
    <SiteHeader />

    <div class="main-container">
      <SideBar />

      <!-- Main content area -->
      <div class="main-content">
        <div class="section">
          <div class="container">
            <SimpleUpload />
            <DeleteMeterReadings />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import SimpleUpload from "./components/MeterReadingUpload.vue";
import SiteHeader from "./components/SiteHeader.vue";
import SideBar from "./components/SideBar.vue"; 
import DeleteMeterReadings from "./components/DeleteMeterReadings.vue"; 

export default {
  name: "App",
  components: {
    SimpleUpload,
    SiteHeader,
    SideBar,
    DeleteMeterReadings
  },
};
</script>

<style>
.app-container {
  display: flex;
  flex-direction: column;
  height: 100vh;
}

.main-container {
  display: flex;
  flex: 1; 
}

.main-content {
  flex: 1; 
}

</style>
