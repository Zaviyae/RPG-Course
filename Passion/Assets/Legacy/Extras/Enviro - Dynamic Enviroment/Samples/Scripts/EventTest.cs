using UnityEngine;
using System.Collections;
namespace EnviroSamples
{
public class EventTest : MonoBehaviour {

	void Start ()
	{
		EnviroSky.instance.OnWeatherChanged += (EnviroWeatherPreset type) =>
		{
			Debug.Log("Weather changed to: " + type.Name);
		};

		EnviroSky.instance.OnSeasonChanged += (EnviroSeasons.Seasons season) =>
		{
			Debug.Log("Season changed");
		};

		EnviroSky.instance.OnHourPassed += () =>
		{
			Debug.Log("Hour Passed!");
		};

		EnviroSky.instance.OnDayPassed += () =>
		{
			Debug.Log("New Day!");
		};
		EnviroSky.instance.OnYearPassed += () =>
		{
			Debug.Log("New Year!");
		};

		EnviroSky.instance.OnZoneChanged += (EnviroZone z) =>
		{
			Debug.Log("ChangedZone: " + z.zoneName);
		};

	}



	public void TestEventsWWeather ()
	{
		print("Weather Changed though interface!");
	}

	public void TestEventsNight ()
	{
		print("Night now!!");
	}

	public void TestEventsDay ()
	{
		print("Day now!!");
	}
}
}