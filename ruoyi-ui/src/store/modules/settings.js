import defaultSettings from "@/settings";
import { getConfigKey } from "@/api/system/config";

const {
  sideTheme,
  showSettings,
  topNav,
  tagsView,
  fixedHeader,
  sidebarLogo,
  dynamicTitle,
} = defaultSettings;

const storageSetting = JSON.parse(localStorage.getItem("layout-setting")) || "";
const state = {
  title: "",
  theme: storageSetting.theme || "#409EFF",
  sideTheme: storageSetting.sideTheme || sideTheme,
  showSettings: showSettings,
  topNav: storageSetting.topNav === undefined ? topNav : storageSetting.topNav,
  tagsView:
    storageSetting.tagsView === undefined ? tagsView : storageSetting.tagsView,
  fixedHeader:
    storageSetting.fixedHeader === undefined
      ? fixedHeader
      : storageSetting.fixedHeader,
  sidebarLogo:
    storageSetting.sidebarLogo === undefined
      ? sidebarLogo
      : storageSetting.sidebarLogo,
  dynamicTitle:
    storageSetting.dynamicTitle === undefined
      ? dynamicTitle
      : storageSetting.dynamicTitle,
};
const mutations = {
  CHANGE_SETTING: (state, { key, value }) => {
    if (state.hasOwnProperty(key)) {
      state[key] = value;
    }
  },
};

const actions = {
  // 修改布局设置
  changeSetting({ commit }, data) {
    commit("CHANGE_SETTING", data);
  },
  // 设置网页标题
  setTitle({ commit }, title) {
    state.title = title;
  },
  async initServerThemeSettings({ commit }) {
    try {
      const { msg: skinName } = await getConfigKey("sys.index.skinName");
      const { msg: sideTheme } = await getConfigKey("sys.index.sideTheme");

      //蓝色 skin-blue、绿色 skin-green、紫色 skin-purple、红色 skin-red、黄色 skin-yellow
      let color = "#409EFF";
      if (skinName && skinName.startsWith("skin")) {
        switch (skinName) {
          case "skin-blue":
            color = "#409EFF";
            break;
          case "skin-green":
            color = "#67C23A";
            break;
          case "skin-purple":
            color = "#7928CB";
            break;
          case "skin-red":
            color = "#F56C6C";
            break;
          case "skin-yellow":
            color = "#E6A23C";
            break;
          default:
            color = "#409EFF";
            break;
        }
      } else {
        color = skinName;
      }
      commit("CHANGE_SETTING", { key: "theme", value: color });
      commit("CHANGE_SETTING", { key: "sideTheme", value: sideTheme });
    } catch {
      console.error("初始化服务端主题失败");
    }
  },
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
};
