import "../App.css";

import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthProvider';



export default function Navbar() {

    const { isLoggedIn, signOut } = useAuth();

    const pageLinks = [
        { name: 'Alle Navne', path: '/navne' },
        { name: 'Vores Matches', path: '/matches', imgUrl: '/heart.svg'},
      ]

    const animationStyles = `
      @keyframes fillChange {
        0% { fill: rgb(255, 0, 0); } /* Red */
        33% { fill: rgb(0, 255, 0); } /* Green */
        67% { fill: rgb(0, 0, 255); } /* Blue */
        100% { fill: rgb(255, 0, 0); } /* Red */
      }
      #Vector {
        animation: fillChange 3s infinite;
      }
    `;

  return (
    <nav className='absolute top-0 w-full flex items-center justify-between border-b py-6 md:px-8 lg:px-28'>
      <div className='uppercase text-sm flex items-center'>
      {/* <img src={logo} alt="logo" className='pr-10 cursor-pointer' onClick={
        () => {
          window.location.href = '/'
        }
      }/> */}
      <p className="text-lg font-medium tracking-tighter mr-12 " onClick={
        () => {
          window.location.href = '/'
        }}>
          NavneGuiden 2.0
      </p>
        {pageLinks.map(link => (
          <div key={link.name} className='flex gap-4 '>
            <Link key={link.name} to={link.path} className='flex items-center gap-2'>
            {link.imgUrl && <img src={link.imgUrl} alt="heart" className='w-4 h-4 ' />}
            {link.name}
          </Link>
              <div className='h-5 w-px bg-gray-300 mr-4' />            
          </div>
        ))}
        {isLoggedIn ? (
              <div className='space-x-4'>
                <Link to='/profile'>
                Min profil
              </Link>
                <button onClick={signOut} className='border-button'>
                Log ud
              </button>
              <button onClick={
                () => {
                  window.location.href = '/tilknyt-partner'
                }
              } className='border-button'>
                Tilknyt partner
              </button>
              </div>
            ) : (
              <div className='flex '>
                <Link to='/sign-in' className='border-button'>
                Log ind
              </Link>
              </div>
            )}
      </div>
     
    </nav>
  )
}
